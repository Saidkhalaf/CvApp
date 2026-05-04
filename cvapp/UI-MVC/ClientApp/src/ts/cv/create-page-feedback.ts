import { onDomReady } from "../utils/on-dom-ready";

const FORM_SELECTOR = ".js-cv-create-form";
const SUCCESS_ALERT_SELECTOR = ".js-cv-success-alert";

function initSuccessAlertAutoDismiss(): void {
  const alertElements = document.querySelectorAll<HTMLElement>(SUCCESS_ALERT_SELECTOR);
  if (!alertElements.length) return;

  alertElements.forEach((alertElement) => {
    const timeoutText = alertElement.dataset.autoHideMs;
    const timeoutMs = Number.parseInt(timeoutText ?? "2000", 10);
    const delay = Number.isNaN(timeoutMs) ? 2000 : timeoutMs;

    window.setTimeout(() => {
      if (!alertElement.isConnected) return;

      const closeButton = alertElement.querySelector<HTMLButtonElement>(".btn-close");
      if (closeButton) {
        closeButton.click();
        return;
      }

      alertElement.classList.remove("show");
      window.setTimeout(() => {
        alertElement.remove();
      }, 300);
    }, delay);
  });
}
//scroll to the section when page reload 
function initRequestedSectionScroll(): void {
  const form = document.querySelector<HTMLFormElement>(FORM_SELECTOR);
  if (!form) return;

  const params = new URLSearchParams(window.location.search);
  const sectionFromQuery = params.get("section")?.trim();
  const sectionFromHash = window.location.hash ? window.location.hash.substring(1).trim() : "";
  const targetSection = sectionFromQuery || sectionFromHash;
  if (!targetSection) return;

  // Delay longer to allow form data to post and page to fully load
  window.setTimeout(() => {
    window.requestAnimationFrame(() => {
      const target = document.getElementById(targetSection);
      const successAlert = target?.querySelector<HTMLElement>(".js-cv-success-alert");
      const errorAlert = target?.querySelector<HTMLElement>(".alert-danger:not(.validation-summary-valid)");
      const feedbackAlert = successAlert ?? errorAlert;

      if (feedbackAlert) {
        feedbackAlert.scrollIntoView({ behavior: "smooth", block: "center" });
        return;
      }

      target?.scrollIntoView({ behavior: "smooth", block: "start" });
    });
  }, 500); 
}


function initCreatePageFeedback(): void {
  initSuccessAlertAutoDismiss();
  initRequestedSectionScroll();
}

onDomReady(initCreatePageFeedback);

export {};
