import Sortable from 'sortablejs';

/**
 * Werkt de tekstlabels in de tabel bij zonder refresh
 */
function updateLiveTrainingList(form: HTMLFormElement, formData: FormData): void {
    const trainingId = formData.get("Id");
    if (!trainingId) return;

    const targetRow = document.querySelector(`tr[data-bs-target="#edit-training-${trainingId}"]`);
    if (!targetRow) return;

    // Selectie op basis van de js- klassen die we in de HTML hebben gezet
    const nameEl = targetRow.querySelector('.js-training-name-label');
    const yearEl = targetRow.querySelector('.js-training-year-label');
    const instEl = targetRow.querySelector('.js-training-institute-label');
    const examEl = targetRow.querySelector('.js-training-exam-label');

    // Haal de waarden op (let op de exacte naamgeving van de inputs in je partials!)
    const name = formData.get("Name") as string;
    const year = formData.get("Year") as string;
    const institute = formData.get("InstituteOrSchool.Name") as string;
    const exam = formData.get("Exam.Name") as string;

    if (nameEl) nameEl.textContent = name;
    if (yearEl) yearEl.innerHTML = `<i class="bi bi-calendar3 me-1"></i> ${year}`;
    if (instEl) instEl.innerHTML = `<i class="bi bi-building me-1"></i> ${institute || 'Geen instituut'}`;
    if (examEl) examEl.innerHTML = `<i class="bi bi-file-earmark-check me-1"></i> ${exam || 'Geen examen'}`;
}

/**
 * Sends form data to the server via AJAX (fetch)
 */
const performDatabaseSave = async (form: HTMLFormElement): Promise<void> => {
    const formData = new FormData(form);
    const url = form.action;
    const tokenInput = form.querySelector('input[name="__RequestVerificationToken"]') as HTMLInputElement;
    const token = tokenInput ? tokenInput.value : '';

    try {
        const response = await fetch(url, {
            method: 'POST',
            body: formData,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'RequestVerificationToken': token
            }
        });

        const result = await response.json();
        if (response.ok && result.success) {
            console.log("Auto-save successful");

            updateLiveTrainingList(form, formData);
        }
    } catch (error) {
        console.error("Auto-save network error:", error);
    }
};

/**
 * Global debounce timers map to handle multiple forms on one page
 */
const debounceTimers: Map<HTMLFormElement, number> = new Map();

/**
 * Initializes auto-save using Event Delegation.
 * This works even for forms loaded dynamically via AJAX.
 */
const initializeCvAutoSave = (): void => {
    document.addEventListener('input', (event) => {
        const target = event.target as HTMLElement;
        const form = target.closest('.js-cv-create-form') as HTMLFormElement;

        if (!form) return;

        const existingTimer = debounceTimers.get(form);
        if (existingTimer) window.clearTimeout(existingTimer);

        const newTimer = window.setTimeout(() => {
            performDatabaseSave(form);
            debounceTimers.delete(form);
        }, 1000);

        debounceTimers.set(form, newTimer);
    });

    // Also trigger on change for selects/checkboxes
    document.addEventListener('change', (event) => {
        const target = event.target as HTMLElement;
        if (target.tagName === 'SELECT' || target.tagName === 'INPUT') {
            const form = target.closest('.js-cv-create-form') as HTMLFormElement;
            if (form) performDatabaseSave(form);
        }
    });
};

/**
 * Initialiseert het slepen (sorting) van de CV secties
 */
export const initializeCvSorting = (): void => {
    const sortableContainer = document.getElementById('cv-sections-container');

    if (!sortableContainer) return;

    new Sortable(sortableContainer, {
        animation: 150,
        handle: '.js-drag-handle',
        draggable: '> .js-sortable-item',
        direction: 'vertical',
        ghostClass: 'sortable-ghost',
        chosenClass: 'sortable-chosen',
        dragClass: 'sortable-drag',
        forceFallback: false,
        fallbackOnBody: false,
        scroll: true,
        scrollSensitivity: 80,
        scrollSpeed: 30,
        swapThreshold: 0.3,
        invertSwap: true,

        onStart: () => {
            document.body.classList.add('is-dragging');
        },
        onEnd: (evt) => {
            document.body.classList.remove('is-dragging');

            // Controleer of de index echt veranderd is
            if (evt.oldIndex !== evt.newIndex) {
                console.log(`Succesvol verplaatst: ${evt.oldIndex} -> ${evt.newIndex}`);
                // Hier kun je je database update aanroepen
            }
        }
    });
};
document.addEventListener('DOMContentLoaded', () => {
    initializeCvAutoSave();
    initializeCvSorting();
});