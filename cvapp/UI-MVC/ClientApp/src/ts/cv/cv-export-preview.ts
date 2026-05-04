type Nullable<T> = T | null;

class CvExportLivePreview {
    private readonly form: HTMLFormElement;
    private readonly previewFrame: HTMLIFrameElement;
    private readonly previewStatus: HTMLElement;
    private readonly emptyState: Nullable<HTMLElement>;

    private currentPreviewUrl: Nullable<string> = null;
    private previewTimeout: number | null = null;

    constructor(
        form: HTMLFormElement,
        previewFrame: HTMLIFrameElement,
        previewStatus: HTMLElement,
        emptyState: Nullable<HTMLElement>
    ) {
        this.form = form;
        this.previewFrame = previewFrame;
        this.previewStatus = previewStatus;
        this.emptyState = emptyState;
    }

    public init(): void {
        // Load preview automatically when the page opens
        void this.loadPreview();

        // Update preview when user changes template, logo or checkboxes
        this.form.addEventListener("change", () => {
            this.schedulePreview();
        });

        window.addEventListener("beforeunload", () => {
            this.revokeCurrentPreviewUrl();
        });
    }

    private schedulePreview(): void {
        if (this.previewTimeout !== null) {
            window.clearTimeout(this.previewTimeout);
        }

        this.setStatus("Wijzigingen gedetecteerd...");

        this.previewTimeout = window.setTimeout(() => {
            void this.loadPreview();
        }, 1200);
    }

    private async loadPreview(): Promise<void> {
        const previewUrl = this.form.dataset.previewUrl;

        if (!previewUrl) {
            this.setErrorStatus("Preview URL ontbreekt");
            return;
        }

        try {
            this.setStatus("Preview wordt geladen...");

            const formData = new FormData(this.form);

            const response = await fetch(previewUrl, {
                method: "POST",
                body: formData,
                credentials: "same-origin"
            });

            if (!response.ok) {
                this.setErrorStatus(`Preview fout: ${response.status}`);
                return;
            }

            const pdfBlob = await response.blob();

            if (pdfBlob.size === 0) {
                this.setErrorStatus("Preview is leeg");
                return;
            }

            const newPreviewUrl = URL.createObjectURL(pdfBlob);

            this.revokeCurrentPreviewUrl();

            this.currentPreviewUrl = newPreviewUrl;
            this.previewFrame.src = newPreviewUrl;

            this.previewFrame.classList.remove("d-none");

            if (this.emptyState) {
                this.emptyState.classList.add("d-none");
            }

            const time = new Date().toLocaleTimeString();
            this.setSuccessStatus(`Bijgewerkt om ${time}`);
        } catch (error) {
            console.error("Preview error:", error);
            this.setErrorStatus("Preview laden mislukt");
        }
    }

    private setStatus(message: string): void {
        this.previewStatus.textContent = message;
        this.previewStatus.className = "badge text-bg-light border";
    }

    private setSuccessStatus(message: string): void {
        this.previewStatus.textContent = message;
        this.previewStatus.className = "badge text-bg-success";
    }

    private setErrorStatus(message: string): void {
        this.previewStatus.textContent = message;
        this.previewStatus.className = "badge text-bg-danger";
    }

    private revokeCurrentPreviewUrl(): void {
        if (this.currentPreviewUrl) {
            URL.revokeObjectURL(this.currentPreviewUrl);
            this.currentPreviewUrl = null;
        }
    }
}

document.addEventListener("DOMContentLoaded", () => {
    const form = document.querySelector<HTMLFormElement>("#cvExportForm");
    const previewFrame = document.querySelector<HTMLIFrameElement>("#pdfPreviewFrame");
    const previewStatus = document.querySelector<HTMLElement>("#previewStatus");
    const emptyState = document.querySelector<HTMLElement>("#previewEmptyState");

    if (!form || !previewFrame || !previewStatus) {
        console.error("Niet alle preview elementen zijn gevonden.");
        return;
    }

    new CvExportLivePreview(
        form,
        previewFrame,
        previewStatus,
        emptyState
    ).init();
});