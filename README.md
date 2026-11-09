# Task 1 — resilient form checks (C# + Selenium)

Small console app for checking First Name, Gender and State on the configurable form page.

## Requirements

- .NET 8 SDK
- Google Chrome
- Network access on first run so NuGet and Selenium Manager can obtain dependencies/driver

## Run

```bash
FORM_URL="http://app.cloudqa.io/home/AutomationPracticeForm" FIRST_NAME="Jayanth" GENDER="Male" STATE="California" dotnet run
```

The URL and values are optional environment variables. The program resolves controls from visible labels, associated labels, fieldset legends, ARIA labels and nearby label context. It scores candidates and stops on a tie instead of choosing a similar-looking field arbitrarily. Reordering and changing id/name/class do not matter while the accessible label relationship remains intact.

## Approach and limits

This is heuristic semantic matching, not a guarantee against arbitrary redesign. It may fail safely if labels disappear or become ambiguous; shadow roots and iframes may need an adapter. It assumes a native/select-like State control and a selectable radio for gender. It does not submit the form or assert server-side persistence. The live form could not be verified in the preparation environment, and .NET was unavailable there, so compile and run locally before relying on it.
