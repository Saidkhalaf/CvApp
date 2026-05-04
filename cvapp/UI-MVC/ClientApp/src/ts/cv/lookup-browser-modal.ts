import { Modal } from "bootstrap";
import { onDomReady } from "../utils/on-dom-ready";

type LookupKind = "education" | "institute" | "certificate" ;

type EducationLookupRow = {
  id: number;
  name: string;
  educationLevel: string;
  eqfLevel: number;
  isQualified: boolean;
};
type CertificateLookupRow = {
  id: number;
  name: string;
  type: string;
};

type InstituteLookupRow = {
  id: number;
  name: string;
  type: string;
  email: string;
  website: string;
  telephoneNumber: string ; 
  btwNumber : string  ;
};

type LookupRow = EducationLookupRow | InstituteLookupRow | CertificateLookupRow;

type LookupField = {
  root: HTMLElement;
  kind: LookupKind;
  hiddenInput: HTMLInputElement;
  displayElement: HTMLElement;
  placeholder: string;
};

type LookupColumn = {
  header: string;
  value: (row: LookupRow) => string;
};

type LookupConfig = {
  title: string;
  description: string;
  badge: string;
  emptyMessage: string;
  columns: LookupColumn[];
};



type RawLookupRow = Record<string, unknown>;

const LOOKUP_CONFIG: Record<LookupKind, LookupConfig> = {
  education: {
    title: "Opleiding kiezen",
    description: "Zoek snel in  opleidingen en koppel de juiste opleiding aan dit CV-item.",
    badge: "Opleidingen",
    emptyMessage: "Geen opleidingen gevonden. Pas je zoekterm aan of voeg een nieuwe opleiding toe.",
    columns: [
      { header: "Naam", value: (row) => ("name" in row ? row.name : "") },
      { header: "Niveau", value: (row) => ("educationLevel" in row ? row.educationLevel : "") },
      { header: "EQF", value: (row) => ("eqfLevel" in row ? row.eqfLevel.toString() : "") },
      { header: "Gekwalificeerd", value: (row) => ("isQualified" in row ? (row.isQualified ? "Ja" : "Nee") : "") }
    ]
  },
  institute: {
    title: "Instituut of school kiezen",
    description: "Zoek snel in alle instituten en scholen en koppel de juiste organisatie aan dit CV-item.",
    badge: "Instituten en scholen",
    emptyMessage: "Geen instituten of scholen gevonden. Pas je zoekterm aan of voeg een nieuw instituut toe.",
    columns: [
      { header: "Naam", value: (row) => ("name" in row ? row.name : "") },
      { header: "Type", value: (row) => ("type" in row ? row.type : "") },
      { header: "E-mail", value: (row) => ("email" in row ? row.email : "") },
      { header: "Website", value: (row) => ("website" in row ? row.website : "") },
      { header: "TelephoneNumber", value: (row) => ("telephoneNumber" in row ? row.telephoneNumber : "") },
      { header: "BtwNumber", value: (row) => ("btwNumber" in row ? row.btwNumber : "") },
    ]
  },
  certificate: {
    title: "Certificaat kiezen",
    description: "Zoek snel in alle certificaten en koppel het juiste certificaat aan dit CV-item.",
    badge: "Certificaten",
    emptyMessage: "Geen certificaten gevonden. Pas je zoekterm aan of voeg een nieuw certificaat toe.",
    columns: [
      { header: "Naam", value: (row) => ("name" in row ? row.name : "") },
      { header: "Type", value: (row) => ("type" in row ? row.type : "") }
    ]
  }

};

function normalize(value: string): string {
  return value.trim().toLocaleLowerCase();
}

function readStringValue(row: RawLookupRow, camelCaseKey: string, pascalCaseKey: string): string {
  const value = row[camelCaseKey] ?? row[pascalCaseKey];
  return typeof value === "string" ? value : "";
}

function readNumberValue(row: RawLookupRow, camelCaseKey: string, pascalCaseKey: string): number {
  const value = row[camelCaseKey] ?? row[pascalCaseKey];
  return typeof value === "number" ? value : 0;
}

function readBooleanValue(row: RawLookupRow, camelCaseKey: string, pascalCaseKey: string): boolean {
  const value = row[camelCaseKey] ?? row[pascalCaseKey];
  return typeof value === "boolean" ? value : false;
}

function mapEducationRow(row: RawLookupRow): EducationLookupRow {
  return {
    id: readNumberValue(row, "id", "Id"),
    name: readStringValue(row, "name", "Name"),
    educationLevel: readStringValue(row, "educationLevel", "EducationLevel"),
    eqfLevel: readNumberValue(row, "eqfLevel", "EqfLevel"),
    isQualified: readBooleanValue(row, "isQualified", "IsQualified")
  };
}

function mapInstituteRow(row: RawLookupRow): InstituteLookupRow {
  return {
    id: readNumberValue(row, "id", "Id"),
    name: readStringValue(row, "name", "Name"),
    type: readStringValue(row, "type", "Type"),
    email: readStringValue(row, "email", "Email"),
    website: readStringValue(row, "website", "Website"),
    telephoneNumber: readStringValue(row,"telephoneNumber", "TelephoneNumber"),
    btwNumber: readStringValue(row, "btwNumber", "BtwNumber"),
  };
}

function mapCertificateRow(row: RawLookupRow): CertificateLookupRow {
  return {
    id: readNumberValue(row, "id", "Id"),
    name: readStringValue(row, "name", "Name"),
    type: readStringValue(row, "type", "Type")
  };
}


function readLookupRows(kind: "education"): EducationLookupRow[];
function readLookupRows(kind: "institute"): InstituteLookupRow[];
function readLookupRows(kind: "certificate"): CertificateLookupRow[];
function readLookupRows(kind: LookupKind): LookupRow[] {
  const source = document.querySelector<HTMLScriptElement>(`script[data-lookup-source='${kind}']`);
  if (!source?.textContent) {
    return [];
  }

  try {
    const rows = JSON.parse(source.textContent) as RawLookupRow[];
    if (!Array.isArray(rows)) {
      return [];
    }

    if (kind === "education") return rows.map(mapEducationRow);
    if (kind === "institute") return rows.map(mapInstituteRow);
    else {
      return rows.map(mapCertificateRow);
    }
  } catch {
    return [];
  }
}

function collectLookupFields(): LookupField[] {
  return Array.from(document.querySelectorAll<HTMLElement>("[data-lookup-field]")).map((root) => {
    const kind = root.dataset.lookupKind as LookupKind | undefined;
    const hiddenInput = root.querySelector<HTMLInputElement>("[data-lookup-value]");
    const displayElement = root.querySelector<HTMLElement>("[data-lookup-display]");
    const placeholder = displayElement?.dataset.lookupPlaceholder?.trim() ?? "";

    if (!kind || !hiddenInput || !displayElement) {
      throw new Error("A lookup field is missing required elements.");
    }

    return {
      root,
      kind,
      hiddenInput,
      displayElement,
      placeholder
    };
  });
}

class LookupBrowserModalController {
  private readonly rowsByKind: Record<LookupKind, LookupRow[]>;
  private readonly fields: LookupField[];
  private readonly modalElement: HTMLElement;
  private readonly modal: Modal;
  private readonly titleElement: HTMLElement;
  private readonly descriptionElement: HTMLElement;
  private readonly badgeElement: HTMLElement;
  private readonly countElement: HTMLElement;
  private readonly searchInput: HTMLInputElement;
  private readonly clearButton: HTMLButtonElement;
  private readonly emptyElement: HTMLElement;
  private readonly headElement: HTMLTableSectionElement;
  private readonly bodyElement: HTMLTableSectionElement;

  private activeField: LookupField | null = null;

  constructor() {
    const modalElement = document.getElementById("cvLookupBrowserModal");
    const titleElement = document.querySelector<HTMLElement>("[data-lookup-modal-title]");
    const descriptionElement = document.querySelector<HTMLElement>("[data-lookup-modal-description]");
    const badgeElement = document.querySelector<HTMLElement>("[data-lookup-modal-badge]");
    const countElement = document.querySelector<HTMLElement>("[data-lookup-modal-count]");
    const searchInput = document.querySelector<HTMLInputElement>("[data-lookup-modal-search]");
    const clearButton = document.querySelector<HTMLButtonElement>("[data-lookup-modal-clear]");
    const emptyElement = document.querySelector<HTMLElement>("[data-lookup-modal-empty]");
    const headElement = document.querySelector<HTMLTableSectionElement>("[data-lookup-modal-head]");
    const bodyElement = document.querySelector<HTMLTableSectionElement>("[data-lookup-modal-body]");

    if (!modalElement || !titleElement || !descriptionElement || !badgeElement || !countElement || !searchInput || !clearButton || !emptyElement || !headElement || !bodyElement) {
      throw new Error("Lookup browser modal is not fully configured.");
    }

    this.modalElement = modalElement;
    this.modal = Modal.getOrCreateInstance(modalElement);
    this.titleElement = titleElement;
    this.descriptionElement = descriptionElement;
    this.badgeElement = badgeElement;
    this.countElement = countElement;
    this.searchInput = searchInput;
    this.clearButton = clearButton;
    this.emptyElement = emptyElement;
    this.headElement = headElement;
    this.bodyElement = bodyElement;
    this.fields = collectLookupFields();
    this.rowsByKind = {
      education: readLookupRows("education"),
      institute: readLookupRows("institute"),
      certificate: readLookupRows("certificate")
    };
  }

  public init(): void {
    if (!this.fields.length) {
      return;
    }

    this.fields.forEach((field) => {
      this.syncField(field);
    });

    this.modalElement.addEventListener("show.bs.modal", (event) => {
      const modalEvent = event as Event & { relatedTarget?: EventTarget | null };
      const trigger = modalEvent.relatedTarget as HTMLElement | null;
      const fieldRoot = trigger?.closest<HTMLElement>("[data-lookup-field]");
      if (!fieldRoot) {
        return;
      }

      const field = this.fields.find((entry) => entry.root === fieldRoot);
      if (!field) {
        return;
      }

      this.open(field);
    });

    this.searchInput.addEventListener("input", () => {
      this.renderResults();
    });

    this.clearButton.addEventListener("click", () => {
      this.clearSelection();
    });

    this.modalElement.addEventListener("hidden.bs.modal", () => {
      this.resetModal();
    });
  }

  private open(field: LookupField): void {
    this.activeField = field;
    const config = LOOKUP_CONFIG[field.kind];

    this.titleElement.textContent = config.title;
    this.descriptionElement.textContent = config.description;
    this.badgeElement.textContent = config.badge;
    this.emptyElement.textContent = config.emptyMessage;
    this.searchInput.value = "";

    this.renderHead(field.kind);
    this.renderResults();

    window.setTimeout(() => {
      this.searchInput.focus();
      this.searchInput.select();
    }, 100);
  }

  private renderHead(kind: LookupKind): void {
    this.headElement.replaceChildren();

    const row = document.createElement("tr");
    LOOKUP_CONFIG[kind].columns.forEach((column) => {
      const cell = document.createElement("th");
      cell.scope = "col";
      cell.textContent = column.header;
      row.appendChild(cell);
    });

    this.headElement.appendChild(row);
  }

  private renderResults(): void {
    if (!this.activeField) {
      return;
    }

    const rows = this.filterRows(this.activeField.kind, this.searchInput.value);
    this.bodyElement.replaceChildren();

    rows.forEach((row) => {
      this.bodyElement.appendChild(this.createRow(row, this.activeField!));
    });

    this.countElement.textContent = `${rows.length} resultaat${rows.length === 1 ? "" : "en"}`;
    this.emptyElement.hidden = rows.length > 0;
    this.bodyElement.hidden = rows.length === 0;
  }

  private filterRows(kind: LookupKind, searchTerm: string): LookupRow[] {
    const normalizedSearchTerm = normalize(searchTerm);
    const rows = this.rowsByKind[kind];

    if (!normalizedSearchTerm) {
      return rows;
    }

    return rows.filter((row) => {
      return LOOKUP_CONFIG[kind].columns
        .some((column) => normalize(column.value(row)).includes(normalizedSearchTerm));
    });
  }

  private createRow(row: LookupRow, field: LookupField): HTMLTableRowElement {
    const tableRow = document.createElement("tr");
    tableRow.className = "cv-lookup-modal__row";
    tableRow.tabIndex = 0;

    if (field.hiddenInput.value.trim() === row.id.toString()) {
      tableRow.classList.add("is-selected");
    }

    LOOKUP_CONFIG[field.kind].columns.forEach((column) => {
      const cell = document.createElement("td");
      const value = column.value(row).trim();
      if (column.header === "Naam") {
        const primary = document.createElement("div");
        primary.className = "cv-lookup-modal__primary";
        primary.textContent = value.length > 0 ? value : "-";
        cell.appendChild(primary);
      } else {
        cell.textContent = value.length > 0 ? value : "-";
      }
      tableRow.appendChild(cell);
    });

    tableRow.addEventListener("click", () => {
      this.applySelection(field, row);
    });

    tableRow.addEventListener("keydown", (event) => {
      if (event.key !== "Enter" && event.key !== " ") {
        return;
      }

      event.preventDefault();
      this.applySelection(field, row);
    });

    return tableRow;
  }

  private applySelection(field: LookupField, row: LookupRow): void {
    field.hiddenInput.value = row.id.toString();
    field.hiddenInput.dispatchEvent(new Event("change", { bubbles: true }));
    this.syncField(field);
    this.modal.hide();
  }

  private clearSelection(): void {
    if (!this.activeField) {
      return;
    }

    this.activeField.hiddenInput.value = "";
    this.activeField.hiddenInput.dispatchEvent(new Event("change", { bubbles: true }));
    this.syncField(this.activeField);
    this.modal.hide();
  }

  private syncField(field: LookupField): void {
    const row = this.rowsByKind[field.kind].find((entry) => entry.id.toString() === field.hiddenInput.value.trim());
    const hasSelection = !!row;

    field.displayElement.textContent = hasSelection ? row.name : field.placeholder;
    field.displayElement.classList.toggle("is-empty", !hasSelection);
    field.root.classList.toggle("is-filled", hasSelection);
  }

  private resetModal(): void {
    this.activeField = null;
    this.searchInput.value = "";
    this.countElement.textContent = "";
    this.headElement.replaceChildren();
    this.bodyElement.replaceChildren();
    this.bodyElement.hidden = false;
    this.emptyElement.hidden = true;
  }
}

onDomReady(() => {
  if (!document.querySelector("[data-lookup-field]")) {
    return;
  }

  const controller = new LookupBrowserModalController();
  controller.init();
});
