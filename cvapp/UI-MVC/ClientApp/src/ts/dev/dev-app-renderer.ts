function renderDevAppTips(): void {
  const appElement = document.querySelector<HTMLDivElement>("#app");
  if (!appElement) return;

  appElement.innerHTML = `
    <main class="container py-5">
      <div class="row justify-content-center">
        <div class="col-12 col-lg-9 col-xl-8">
          <div class="card border-0 shadow-sm">
            <div class="card-body p-4 p-md-5">
              <div class="d-flex align-items-center mb-3">
                <i class="bi bi-tools text-primary fs-3 me-2" aria-hidden="true"></i>
                <h1 class="h3 mb-0">ClientApp Dev Page</h1>
              </div>

              <p class="text-body-secondary mb-4">This page is for frontend asset development only.</p>

              <div class="alert alert-info d-flex" role="alert">
                <i class="bi bi-info-circle-fill me-2" aria-hidden="true"></i>
                <div>Use <code>https://localhost:7259</code> for the real MVC Razor app.</div>
              </div>

              <h2 class="h6 text-uppercase text-body-secondary mt-4">Dev Workflow</h2>
              <ol class="mb-0 ps-3">
                <li>Change files in <code>UI-MVC/ClientApp/src</code>.</li>
                <li>Create new TS entries in a dedicated folder: <code>src/ts/&lt;feature-name&gt;/&lt;feature-name&gt;.ts</code>.</li>
                <li>Add each new entry to <code>ClientApp/src/ts/main.ts</code>.</li>
                <li>Build assets with <code>dotnet build</code> from the solution root.</li>
                <li>Refresh the MVC app page.</li>
              </ol>
            </div>
          </div>
        </div>
      </div>
    </main>
  `;
}

renderDevAppTips();

