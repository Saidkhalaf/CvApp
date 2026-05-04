type BootstrapGlobal = {
  Toast: new (el: Element, options?: { autohide?: boolean; delay?: number }) => { show: () => void };
};

function initToastAutoShow(): void {
  const bootstrapGlobal = (window as unknown as { bootstrap?: BootstrapGlobal }).bootstrap;
  if (!bootstrapGlobal) return;

  const successToastEl = document.getElementById("successToast");
  if (successToastEl) {
    const successToast = new bootstrapGlobal.Toast(successToastEl, {
      autohide: true,
      delay: 2000
    });
    successToast.show();
  }

  const errorToastEl = document.getElementById("errorToast");
  if (errorToastEl) {
    const errorToast = new bootstrapGlobal.Toast(errorToastEl, {
      autohide: true,
      delay: 2000
    });
    errorToast.show();
  }
}

if (document.readyState === "loading") {
  document.addEventListener("DOMContentLoaded", initToastAutoShow, { once: true });
} else {
  initToastAutoShow();
}

