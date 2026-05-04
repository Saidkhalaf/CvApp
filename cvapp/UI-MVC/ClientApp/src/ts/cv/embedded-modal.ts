import { Modal } from "bootstrap";
import { onDomReady } from "../utils/on-dom-ready";

const EMBEDDED_CANCEL_SELECTOR = ".js-embedded-cancel"; // find all the links with cancel
const EMBEDDED_MODAL_SELECTOR = ".cv-modal-blur";

//detects cancel links inside the iframe 
function wireCancelLinks(modalElement: HTMLElement, iframe: HTMLIFrameElement): void {
  const iframeDocument = iframe.contentDocument;
  if (!iframeDocument) return;

  const cancelLinks = iframeDocument.querySelectorAll<HTMLAnchorElement>(EMBEDDED_CANCEL_SELECTOR);
  cancelLinks.forEach((link) => {
    if (link.dataset.cvModalBound === "1") return;
    link.dataset.cvModalBound = "1";

    link.addEventListener("click", (event) => {
      const href = link.getAttribute("href");
      if (!href) return;

      const destination = new URL(href, window.location.origin);
      const current = new URL(window.location.href);
      const isSameParentPage =
        destination.origin === current.origin &&
        destination.pathname === current.pathname;

      if (!isSameParentPage) return;

      event.preventDefault();
      Modal.getOrCreateInstance(modalElement).hide();

      const destinationUrl = `${destination.pathname}${destination.search}${destination.hash}`;
      const currentUrl = `${current.pathname}${current.search}${current.hash}`;

      if (destinationUrl !== currentUrl) {
        window.setTimeout(() => {
          window.location.assign(destination.toString());
        }, 150);
      }
    });
  });
}

//sets up modals on the page
function initEmbeddedModals(): void {
  const modalElements = document.querySelectorAll<HTMLElement>(EMBEDDED_MODAL_SELECTOR);
  modalElements.forEach((modalElement) => {
    const iframe = modalElement.querySelector<HTMLIFrameElement>(".cv-embed-frame");
    if (!iframe) return;

    iframe.addEventListener("load", () => {
      wireCancelLinks(modalElement, iframe);
    });
    wireCancelLinks(modalElement, iframe);

    modalElement.addEventListener("show.bs.modal", (event) => {
      const modalEvent = event as Event & { relatedTarget?: EventTarget | null };
      const trigger = modalEvent.relatedTarget as HTMLElement | null;
      if (!trigger) return;

      const modalSrc = trigger.getAttribute("data-modal-src");
      if (modalSrc) {
        iframe.src = modalSrc;
        modalElement.dataset.dynamicModalSource = "1";
      } else {
        delete modalElement.dataset.dynamicModalSource;
      }

      const modalTitle = trigger.getAttribute("data-modal-title");
      if (modalTitle) {
        const titleElement = modalElement.querySelector<HTMLElement>(".modal-title");
        if (titleElement) titleElement.textContent = modalTitle;
      }
    });

    modalElement.addEventListener("hidden.bs.modal", () => {
      if (modalElement.dataset.dynamicModalSource === "1") {
        iframe.src = "about:blank";
        delete modalElement.dataset.dynamicModalSource;
      }
    });
  });
}

onDomReady(initEmbeddedModals);
