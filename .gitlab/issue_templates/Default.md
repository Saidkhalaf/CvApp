## Detailed description

When a student is going through the enrollment process,  
the student must be able to enroll in course units within the program  
for which the student has not yet obtained credits.

---

## Changes to the domain model

The concept **CourseUnitEnrollment** must be added.  
This entity is linked to:
- a student enrollment
- a course unit

---

## Tasks

* [ ] Display a list of course units for which the student has not yet obtained credits  
* [ ] Allow the student to select course units and save the selected course units  
* [ ] Add update functionality  
    * Preselect course units for which the student is already enrolled  
    * Remove enrollments when they are deselected  

---

## Relevant wireframes

Link to the wireframes

---

## Acceptance criteria

```
  Background:
    Given Students
    | name | email      |  id|
    | Jan  | jan@kdg.be |  1 |
    | Piet | piet@kdg.be|  2 |

    Given Courses
    | name          | id  | credits |
    | Programmeren  | 1   | 6       |
    | Databanken    | 2   | 3       |


    Given CourseSubscriptions
    | studentId | courseId | creditReceived   |
    | 1         | 1        | TRUE             |
    | 1         | 2        | FALSE            |

    Given subscription
    | studentId | year | id |
    | 1         | 2023 | 1  |

    Scenario: Student subscribes to a course
      Given student with id 1 edits subscription with id 1
      When student adds course with id 2
      Then subscription with id 1 has a courseSubscriptions with courseId 2

    Scenario: Student subscribes to a course he already has
        Given student with id 1 edits subscription with id 1
        When student adds course with id 1
        Then subscription with id 1 has a courseSubscriptions with courseId 1
```

## Definition of ready checklist

- [ ] All conditions in the Definition of Ready checklist (see canvas) have been met.  
- [ ] This user story has been refined.
