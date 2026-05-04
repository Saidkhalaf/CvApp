/**
 * Unified CV Item Delete & Create Handler.
 * Bevat SMART AUTO-SAVE: Creëert de training op de achtergrond zodra verplichte velden
 * correct zijn. Zet zichzelf daarna automatisch om in een Edit-formulier!
 */

declare const $: any;

interface DeleteResponse {
  success: boolean;
  message?: string;
  id?: number; // Nieuw: ID dat we terugkrijgen na creatie
}

type DeleteTrigger = HTMLElement & {
  dataset: DOMStringMap & {
    deleteAction?: string;
    deleteId?: string;
    deleteIdName?: string;
    deleteLabel?: string;
    deleteType?: string;
  };
};

const SELECTORS = {
  MODAL: '#deleteCvItemModal',
  DELETE_FORM: '#deleteCvItemForm',
  TRAINING_LIST: '#trainings-list',
  NEW_TRAINING_COLLAPSE: '#collapseNewTraining',
  MODAL_PROMPT: '#deleteCvItemModal .modal-body p:first-child'
} as const;

let pendingDeleteId: string | null = null;
let pendingDeleteType: string | null = null;
let createAutoSaveTimer: number | null = null;
let inactivityTimer: number | null = null; // NIEUW: Timer voor het sluiten

/**
 * Zorgt ervoor dat er maar één bewerk formulier tegelijk open is.
 */
function initExclusiveCollapses(): void {
  const trainingSection = document.getElementById('cursussen-trainingen');
  if (!trainingSection) return;

  trainingSection.addEventListener('show.bs.collapse', (event) => {
    const target = event.target as HTMLElement;

    if (target.id === 'collapseTrainingSection') return;

    const openCollapses = trainingSection.querySelectorAll('.collapse.show');
    openCollapses.forEach((openCollapse) => {
      if (openCollapse.id === 'collapseTrainingSection') return;
      if (openCollapse === target) return;
      if (openCollapse.contains(target)) return;

      ($(openCollapse) as any).collapse('hide');
    });
  });
}

function getTrainingElements(id: string) {
  const $targetButton = $(`button[data-delete-id="${id}"][data-delete-type="training"]`);
  const $cardItem = $targetButton.closest('.list-group-item, .card, tr');
  const $editCollapseRow = $cardItem.next('tr:has(.collapse), .collapse, tr.collapse-row');

  return { $tableRow: $cardItem, $editCollapseRow };
}

function updateEmptyState(): void {
  const $list = $(SELECTORS.TRAINING_LIST);
  const hasItems = $list.find('button[data-delete-type="training"]').length > 0;

  if (!hasItems) {
    const emptyTemplate = `
      <tr class="empty-placeholder">
        <td colspan="2" class="p-5 text-center text-muted italic">
          <i class="bi bi-info-circle me-2"></i>
          Geen cursussen of trainingen toegevoegd.
        </td>
      </tr>`;

    $list.empty().append(emptyTemplate);

    const collapseEl = document.getElementById('collapseTrainingSection');
    if (collapseEl) {
      const bsCollapse = (window as any).bootstrap?.Collapse;
      bsCollapse?.getOrCreateInstance(collapseEl).show();
    }
  } else {
    $list.find('.empty-placeholder').remove();
  }
}

function reloadTrainingsList(): void {
  const $container = $(SELECTORS.TRAINING_LIST);
  if ($container.length === 0) return;

  $.get(window.location.href, (data: string) => {
    const $newContent = $(data).find(SELECTORS.TRAINING_LIST);
    if ($newContent.length > 0) {
      $container.replaceWith($newContent);
      updateEmptyState();
    }
  });
}

async function removeTrainingFromDOM(id: string): Promise<void> {
  const { $tableRow, $editCollapseRow } = getTrainingElements(id);

  return new Promise((resolve) => {
    if ($tableRow.length === 0) {
      updateEmptyState();
      resolve();
      return;
    }

    $.when(
        $tableRow.fadeOut(300),
        $editCollapseRow.fadeOut(300)
    ).done(() => {
      $tableRow.remove();
      $editCollapseRow.remove();
      updateEmptyState();
      resolve();
    });
  });
}

function initUniversalDeleteModal(): void {
  const modal = document.getElementById("deleteCvItemModal");
  if (!modal) return;

  const form = document.getElementById("deleteCvItemForm") as HTMLFormElement | null;
  const promptElement = document.querySelector(SELECTORS.MODAL_PROMPT);

  if (!form) return;

  modal.addEventListener("show.bs.modal", (event) => {
    const modalEvent = event as Event & { relatedTarget?: EventTarget | null };
    const trigger = modalEvent.relatedTarget as DeleteTrigger | null;
    if (!trigger) return;

    pendingDeleteId = trigger.dataset.deleteId ?? null;
    pendingDeleteType = trigger.dataset.deleteType ?? null;

    if (promptElement) {
      promptElement.textContent = `Weet je zeker dat je deze ${(pendingDeleteType || 'item').toLowerCase()} wilt verwijderen?`;
    }

    form.action = trigger.dataset.deleteAction ?? "";

    const idInput = form.querySelector('#deleteCvItemId') as HTMLInputElement;
    if (idInput) idInput.value = pendingDeleteId ?? "";
  });

  form.addEventListener("submit", async (e) => {
    e.preventDefault();
    e.stopImmediatePropagation();

    const submitBtn = form.querySelector('button[type="submit"]') as HTMLButtonElement;
    if (submitBtn) submitBtn.disabled = true;

    try {
      const response = await fetch(form.action, {
        method: 'POST',
        body: new URLSearchParams(new FormData(form) as any),
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
      });

      const contentType = response.headers.get("content-type");

      if (response.ok && contentType && contentType.includes("application/json")) {
        const data: DeleteResponse = await response.json();
        if (data.success) {
          if (pendingDeleteType === "training" && pendingDeleteId) {
            await removeTrainingFromDOM(pendingDeleteId);
          } else {
            reloadTrainingsList();
          }
          ($(modal) as any).modal('hide');
        } else {
          alert(data.message || "Fout bij verwijderen.");
        }
      } else {
        reloadTrainingsList();
        ($(modal) as any).modal('hide');
      }
    } catch (err) {
      console.error("Delete Error:", err);
    } finally {
      if (submitBtn) submitBtn.disabled = false;
    }
  });
}

// --- SMART AUTO-SAVE LOGICA ---

/**
 * Toont een tijdelijke melding in de lijst.
 */
function showListNotification(message: string): void {
  const $list = $(SELECTORS.TRAINING_LIST);
  const notificationHtml = `
    <tr class="inactivity-notice bg-success-subtle text-success">
      <td colspan="2" class="p-3 text-center small">
        <i class="bi bi-info-circle-fill me-2"></i> ${message}
        <strong>Als je iets wilt bewerken, druk op 
            <i class="bi bi-pencil text-warning"></i> Bewerk!
        </strong>
      </td>
    </tr>`;

  $('.inactivity-notice').remove();
  $list.prepend(notificationHtml);

  setTimeout(() => {
    $('.inactivity-notice').fadeOut(1000, function(this: HTMLElement) {
      $(this).remove();
    });
  }, 8000);
}

/**
 * Start de timer om het formulier na 10 seconden te sluiten.
 */
function startInactivityTimer(form: HTMLFormElement): void {
  if (inactivityTimer) window.clearTimeout(inactivityTimer);

  inactivityTimer = window.setTimeout(() => {
    const collapseEl = document.getElementById('collapseNewTraining');
    // Sluit alleen als de training al is aangemaakt (ID bestaat)
    if (collapseEl && form.dataset.createdTrainingId) {
      ($(collapseEl) as any).collapse('hide');
      showListNotification("Formulier gesloten wegens inactiviteit.");
    }
  }, 10000); // 10 seconden
}

/**
 * Validatie voor de Auto-Save: Checkt of de 3 verplichte velden oké zijn
 */
function isValidCreateTrainingForm(form: HTMLFormElement): boolean {
  const name = (form.querySelector('[name="Name"]') as HTMLInputElement)?.value.trim();
  const yearStr = (form.querySelector('[name="Year"]') as HTMLInputElement)?.value;
  const courseType = (form.querySelector('[name="CourseType"]') as HTMLSelectElement)?.value;

  if (!name || name.length === 0) return false;
  if (!courseType || courseType.length === 0) return false;

  const year = parseInt(yearStr, 10);
  const currentYear = new Date().getFullYear();
  return !(isNaN(year) || year < 1900 || year > currentYear + 10);
}

/**
 * Visuele feedback in het formulier tonen.
 */
function showSaveStatus(form: HTMLFormElement, message: string, type: 'info' | 'success' | 'error' = 'info') {
  const existingStatuses = form.parentElement?.querySelectorAll('.auto-save-status');
  existingStatuses?.forEach(el => el.remove());

  const statusEl = document.createElement('div');
  statusEl.className = `auto-save-status small fw-bold px-3 py-2 rounded d-inline-block transition-fade ms-3 text-${type} bg-${type}-subtle`;
  statusEl.innerHTML = type === 'success' ? `<i class="bi bi-check-circle me-1"></i> ${message}` : message;

  const header = form.parentElement?.querySelector('h5');
  if (header) {
    header.parentElement?.appendChild(statusEl);
  } else {
    form.prepend(statusEl);
  }

  if (type === 'success') {
    setTimeout(() => {
      $(statusEl).fadeOut(1000, function(this: HTMLElement) {
        $(this).remove();
      });
    }, 2000);
  }
}

/**
 * Daadwerkelijke Smart Auto-Save executie
 */
async function executeSmartAutoSave(form: HTMLFormElement): Promise<void> {
  if (!isValidCreateTrainingForm(form)) return;

  showSaveStatus(form, "Opslaan...", "info");

  const formData = new FormData(form);
  const existingId = form.dataset.createdTrainingId;
  const isUpdate = !!existingId;

  const url = isUpdate ? '/Training/AutoSaveEdit' : form.action;

  if (isUpdate) {
    formData.append('Id', existingId!);
  }

  try {
    const response = await fetch(url, {
      method: 'POST',
      body: formData,
      headers: { 'X-Requested-With': 'XMLHttpRequest' }
    });

    if (response.ok) {
      const contentType = response.headers.get("content-type");

      if (contentType && contentType.includes("application/json")) {
        const result = await response.json();
        if (result.success) {
          showSaveStatus(form, "Opgeslagen!", "success");

          if (!isUpdate && result.id) {
            form.dataset.createdTrainingId = result.id.toString();
          }

          reloadTrainingsList();
        } else {
          showSaveStatus(form, "Validatiefout", "error");
        }
      }
    }
  } catch (error) {
    showSaveStatus(form, "Netwerkfout", "error");
  }
}

/**
 * Reset het formulier volledig als de gebruiker het paneel opnieuw opent
 */
function initNewTrainingCollapse(): void {
  const collapseEl = document.getElementById('collapseNewTraining');
  if (!collapseEl) return;

  collapseEl.addEventListener('show.bs.collapse', (event) => {
    if (event.target !== collapseEl) return;

    const form = collapseEl.querySelector('#form-create-training') as HTMLFormElement;
    if (form) {
      form.reset();
      delete form.dataset.createdTrainingId;
      if (inactivityTimer) window.clearTimeout(inactivityTimer);

      const statusEl = form.parentElement?.querySelector('.auto-save-status');
      if (statusEl) statusEl.remove();
    }
  });

  // Stop timer als gebruiker handmatig sluit
  collapseEl.addEventListener('hide.bs.collapse', () => {
    if (inactivityTimer) window.clearTimeout(inactivityTimer);
  });
}

function handleCreateAutoSave(event: Event): void {
  const target = event.target as HTMLElement;
  const form = target.closest('#form-create-training') as HTMLFormElement | null;
  if (!form) return;

  if (createAutoSaveTimer) window.clearTimeout(createAutoSaveTimer);
  if (inactivityTimer) window.clearTimeout(inactivityTimer); // Reset ook de sluit-timer

  createAutoSaveTimer = window.setTimeout(() => {
    executeSmartAutoSave(form);
  }, 1500);

  startInactivityTimer(form); // Start/Reset de 10 seconden timer
}

/**
 * Vangt handmatige "Enters" op
 */
function handleCreateSubmit(event: Event): void {
  const form = event.target as HTMLFormElement;
  if (!form || form.id !== 'form-create-training') return;

  event.preventDefault();
  event.stopImmediatePropagation();

  if (createAutoSaveTimer) window.clearTimeout(createAutoSaveTimer);
  executeSmartAutoSave(form);
}

function initModule(): void {
  const setup = () => {
    initExclusiveCollapses();
    initUniversalDeleteModal();
    initNewTrainingCollapse();

    document.removeEventListener('submit', handleCreateSubmit);
    document.removeEventListener('input', handleCreateAutoSave);
    document.removeEventListener('change', handleCreateAutoSave);

    document.addEventListener('submit', handleCreateSubmit);
    document.addEventListener('input', handleCreateAutoSave);
    document.addEventListener('change', handleCreateAutoSave);
  };

  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", setup);
  } else {
    setup();
  }
}

initModule();

export {};