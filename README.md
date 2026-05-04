# Project Overview: Cv Application

## 1. Context
The organization currently utilizes an internal CV tool that is insufficient in terms of user-friendliness, structure, and maintainability[cite: 2, 3]. [cite_start]This new application is designed to resolve these shortcomings by providing a structured and consistent way for consultants to build, manage, and export their professional CVs.

## 2. Core Objectives
* **Structured Management:** Support for multiple CV sections managed as independent modules.
* **Export Capabilities:** Generate CVs in PDF and .docx formats based on templates, with the ability to include company logos.
* **Multilingual Support:** Full support for multiple languages.
* **Centralized Entity:** All system data is aggregated around a central "Person" entity.

## 3. Functional Concepts
* **Modular Construction:** Instead of a single document, a CV is built from separate modules (sections) linked to a person.
* **Independent Management:** Each module must support independent CRUD (Create, Read, Update, Delete) operations.
* **Shared Values:** Modules utilize shared input values via dropdowns to ensure consistency.

## 4. Technical Expectations
The implementation focuses on correct relationship modeling (e.g., Person → Career → Project), reusable form components, and a maintainable architecture prepared for data export.

****

# Solution Architecture

This project follows a clean, layered N-Tier architecture to ensure separation of concerns and high testability.

## Project Breakdown

### 1. Domain
The core of the system containing enterprise logic and objects.
* **Entities:** Core data models such as `Person` and `CourseTraining`.
* **Enums:** Shared constants like `CourseType` or `Language`.

### 2. DAL (Data Access Layer)
Responsible for database communication.
* Handles data persistence via Entity Framework Core.
* Manages database migrations and repositories.

### 3. BL (Business Layer)
The logic engine of the application.
* **Managers:** Orchestrates business rules (e.g., `TrainingManager`).
* **Validation:** Ensures data integrity before reaching the DAL.

### 4. UI-MVC
The presentation layer for the user.
* **Controllers:** Coordinates between BL and Views.
* **ViewModels:** Data structures optimized for the UI (e.g., `CreateTrainingViewModel`).
* **Views:** Razor templates for the user interface.

### 5. Tests
A comprehensive suite for quality assurance.
* **Unit Tests:** Isolated testing of logic using Moq.
* **Integration Tests:** Verifying the full flow from HTTP requests to database logic using `WebApplicationFactory`.

****

# Feature Documentation: Courses and Training

## User Story
**As a user, I want to create a training record so that I can show additional learning on my CV.**

## Functional Requirements
Based on the project description[cite: 83, 84]:
* Users must be able to manage additional educational records including name, type, and year.
* Training records must be linked to an Institute.
* Support for Exam information and the ability to upload certificates/attestations.
* Full CRUD functionality is required for this module.

## Current Implementation Progress

### 1. Data Modeling
* Implemented `CourseTraining` entity in the **Domain** layer with validation attributes for `Name` (Required, 2-200 chars) and `Year` (Range 1900-2100).
* Implemented `CreateTrainingViewModel` in the **UI-MVC** layer to handle form input and dropdown populating.

### 2. Business Logic
* Developed `TrainingManager` in the **BL** layer to handle the creation and retrieval of training records.

### 3. Testing Status
* **Integration Tests:** Verified that the `/Training/Create` endpoint correctly handles both valid and invalid data. Valid submissions result in a 302 Redirect to the CV overview, while invalid data correctly re-renders the form with error messages.
* **Unit Tests:** * Verified `TrainingController` correctly interacts with the `ITrainingManager`.
    * Confirmed that `TempData` is correctly set for success messages.
    * Validated that the Controller returns the correct View when business logic validation fails.

### 4. UI/UX
* Created the `Create` view with form validation using ASP.NET Core Tag Helpers.
* Integrated dropdown lists for `CourseType`, `Institute`, and `Exams`.