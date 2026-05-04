type Theme = "light" | "dark";

const THEME_STORAGE_KEY = "cvapp.theme";
const rootElement = document.documentElement;
const colorSchemeMediaQuery = window.matchMedia("(prefers-color-scheme: dark)");

function isTheme(value: string | null): value is Theme {
  return value === "light" || value === "dark";
}

function readStoredTheme(): Theme | null {
  try {
    const storedTheme = localStorage.getItem(THEME_STORAGE_KEY);
    return isTheme(storedTheme) ? storedTheme : null;
  } catch {
    return null;
  }
}

function writeStoredTheme(theme: Theme): void {
  try {
    localStorage.setItem(THEME_STORAGE_KEY, theme);
  } catch {
    // Ignore storage failures (private mode / disabled storage).
  }
}

function getPreferredTheme(): Theme {
  const storedTheme = readStoredTheme();
  if (storedTheme) {
    return storedTheme;
  }

  // App default is always light mode unless the user explicitly toggled it.
  return "light";
}

function getCurrentTheme(): Theme {
  const value = rootElement.getAttribute("data-bs-theme");
  return isTheme(value) ? value : "light";
}

function applyTheme(theme: Theme): void {
  rootElement.setAttribute("data-bs-theme", theme);
}

function updateToggleState(theme: Theme): void {
  const toggleButtons = document.querySelectorAll<HTMLButtonElement>("[data-theme-toggle]");
  const isDarkTheme = theme === "dark";

  toggleButtons.forEach((button) => {
    button.setAttribute("aria-pressed", isDarkTheme.toString());

    const icon = button.querySelector<HTMLElement>("[data-theme-icon]");
    if (icon) {
      icon.classList.toggle("bi-moon-stars", !isDarkTheme);
      icon.classList.toggle("bi-sun", isDarkTheme);
    }

    const label = button.querySelector<HTMLElement>("[data-theme-label]");
    if (label) {
      label.textContent = isDarkTheme ? "Licht" : "Donker";
    }
  });
}

function setTheme(theme: Theme, persist: boolean): void {
  applyTheme(theme);
  updateToggleState(theme);

  if (persist) {
    writeStoredTheme(theme);
  }
}

function bindToggle(): void {
  document.querySelectorAll<HTMLButtonElement>("[data-theme-toggle]").forEach((button) => {
    button.addEventListener("click", () => {
      const nextTheme: Theme = getCurrentTheme() === "dark" ? "light" : "dark";
      setTheme(nextTheme, true);
    });
  });
}

function initializeThemeToggle(): void {
  setTheme(getPreferredTheme(), false);

  document.addEventListener("DOMContentLoaded", () => {
    updateToggleState(getCurrentTheme());
    bindToggle();
  });

  colorSchemeMediaQuery.addEventListener("change", (event) => {
    if (!readStoredTheme()) {
      setTheme(event.matches ? "dark" : "light", false);
    }
  });
}

initializeThemeToggle();

