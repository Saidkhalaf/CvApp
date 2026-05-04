const MOBILE_BREAKPOINT_QUERY = "(max-width: 768px)";

function isMobileViewport(): boolean {
  return window.matchMedia(MOBILE_BREAKPOINT_QUERY).matches;
}

function initMobileSidebar(): void {
  const sidebarToggle = document.getElementById("sidebarToggle") as HTMLInputElement | null;
  const sidebarBackdrop = document.querySelector<HTMLElement>(".sidebar-backdrop");

  if (!sidebarToggle || !sidebarBackdrop) return;

  sidebarBackdrop.addEventListener("click", () => {
    sidebarToggle.checked = false;
  });

  const sidebarLinks = document.querySelectorAll<HTMLAnchorElement>("#sidebar a");
  sidebarLinks.forEach((link) => {
    link.addEventListener("click", () => {
      if (!isMobileViewport()) return;

      // Keep hash and navigation behavior first, then close the drawer.
      window.requestAnimationFrame(() => {
        sidebarToggle.checked = false;
      });
    });
  });
}

if (document.readyState === "loading") {
  document.addEventListener("DOMContentLoaded", initMobileSidebar, { once: true });
} else {
  initMobileSidebar();
}


