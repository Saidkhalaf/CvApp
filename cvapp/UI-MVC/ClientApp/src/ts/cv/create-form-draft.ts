import { onDomReady } from "../utils/on-dom-ready";

type FormField = HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement;
type DraftPayload = Record<string, string>;

interface DraftConfig {
  formSelector: string;
  previousSelector?: string;
  leaveLinkSelector?: string;
  storagePrefix?: string;
}

//create unique storage key based on form action
// example cv:create:draft:/cvEducation/create
function getStorageKey(form: HTMLFormElement, prefix: string): string {
  const action = form.getAttribute("action") || window.location.pathname;
  return `${prefix}${action}`;
}

//get all inputs/select/textarea fields in the form
function getTrackedFields(form: HTMLFormElement): FormField[] {
  return Array.from(form.querySelectorAll<FormField>("input, select, textarea")).filter((field) => {
    if (!field.name) return false;
    if (field.type === "hidden" && !field.hasAttribute("data-lookup-value")) return false;
    if (field.type === "file") return false;
    if (field.name === "__RequestVerificationToken") return false;
    return !field.hasAttribute("disabled");
  });
}

//saves all trackfield value to session storage as json string.
// runs on form input/change/submit,
function writeDraft(form: HTMLFormElement, prefix: string): void {
  const payload: DraftPayload = {};
  for (const field of getTrackedFields(form)) {
    payload[field.name] = field.value;
  }
  sessionStorage.setItem(getStorageKey(form, prefix), JSON.stringify(payload));
}

//reads the saved draft from session storage and restores the field values
function restoreDraft(form: HTMLFormElement, prefix: string): void {
  const raw = sessionStorage.getItem(getStorageKey(form, prefix));
  if (!raw) return;

  let draft: DraftPayload;
  try {
    draft = JSON.parse(raw) as DraftPayload;
  } catch {
    return;
  }

  for (const field of getTrackedFields(form)) {
    const value = draft[field.name];
    if (value === undefined) continue;
    try {
      field.value = value;
    } catch {
      // Ignore non-restorable controls and continue restoring the rest.
    }
  }
}

//binds the writeDraft function to form input, change and submit events, as well as to previous button click and leave links click
// auto save
function bindAutoSave(form: HTMLFormElement, config: DraftConfig): void {
  const prefix = config.storagePrefix || "draft:";
  const persist = () => writeDraft(form, prefix);
  form.addEventListener("input", persist); //typing
  form.addEventListener("change", persist);// dropdown
  form.addEventListener("submit", persist); // from submit button

  if (config.previousSelector) {
    const previousButton = document.querySelector<HTMLButtonElement>(config.previousSelector);
    if (previousButton) {
      previousButton.addEventListener("click", () => {
        persist();
        const firstSection = document.getElementById("persoon-gegevens");
        firstSection?.scrollIntoView({ behavior: "smooth", block: "start" });
      });
    }
  }

  if (config.leaveLinkSelector) {
    const leaveLinks = document.querySelectorAll<HTMLAnchorElement>(config.leaveLinkSelector);
    leaveLinks.forEach((link) => {
      link.addEventListener("click", () => persist());
    });
  }
}

function initFormDraft(config: DraftConfig): void {
  const form = document.querySelector<HTMLFormElement>(config.formSelector);
  if (!form) return;

  const prefix = config.storagePrefix || "draft:";
  restoreDraft(form, prefix);
  bindAutoSave(form, config);
}

onDomReady(() => {
  initFormDraft({
    formSelector: ".js-cv-create-form",
    previousSelector: ".js-cv-previous",
    leaveLinkSelector: ".js-cv-leave-link",
    storagePrefix: "cv:create:draft:"
  });
});

export { initFormDraft, type DraftConfig };
