const DELETE_MODAL_ID = "deleteCvEducationModal";

function initCvEducationDeleteModal(): void {
  const deleteModal = document.getElementById(DELETE_MODAL_ID);
  if (!deleteModal) return;

  deleteModal.addEventListener("show.bs.modal", (event) => {
    const modalEvent = event as Event & { relatedTarget?: EventTarget | null };
    const trigger = modalEvent.relatedTarget as HTMLElement | null;
    if (!trigger) return;

    const id = trigger.getAttribute("data-cveducation-id") ?? "";
    const name = trigger.getAttribute("data-cveducation-name") ?? "";
    const period = trigger.getAttribute("data-cveducation-period") ?? "";

    const idInput = document.getElementById("deleteCvEducationId") as HTMLInputElement | null;
    const nameElement = document.getElementById("deleteCvEducationName");
    const periodElement = document.getElementById("deleteCvEducationPeriod");

    if (idInput) idInput.value = id;
    if (nameElement) nameElement.textContent = name;
    if (periodElement) periodElement.textContent = period;
  });
}

if (document.readyState === "loading") {
  document.addEventListener("DOMContentLoaded", initCvEducationDeleteModal, { once: true });
} else {
  initCvEducationDeleteModal();
}

export {};


