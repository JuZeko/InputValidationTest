# Validation tests

To run your tests in Package Manager Console use the following command:

```bash
dotnet test --settings test.runsettings
```

To run your tests in Terminal use the following command:
```bash
 npx playwright test --ui
```

#### Input Validation
As a personal challenge, I decided to find a bug in every form element using Playwright test framework using TS or C#.

## Identified Issues

1. **First name Input**
   - **Issue**: Error with First name
   - **Details**: The first name input field accepts 3 letters, but submitting the form results in a 500 error.

2. **Last name Input**
   - **Issue**: Error with Last name
   - **Details**: The last name input field can be left empty, but the form still submits.

3. **Country Dropdown**
   - **Issue**: Incorrect Country Name
   - **Details**: The dropdown selection shows error label Bad country: Eswatini (fmr. "Swaziland").

4. **Age Input**
   - **Issue**: Error with Decimal Values
   - **Details**: The input field does not correctly handle decimal values (e.g., "19.00").

5. **Notes Textarea**
   - **Issue**: Lack of XSS Protection
   - **Details**: The textarea does not protect against Cross-Site Scripting (XSS) attacks; all input is vulnerable.



## Setup

If you want to run via test explorer you have to do this because somehow it not detect test.runsettings file for the first time 

[Official Microsoft documentation - Manually select the run settings file](https://learn.microsoft.com/en-us/visualstudio/test/configure-unit-tests-by-using-a-dot-runsettings-file?view=vs-2022).

![image](https://github.com/user-attachments/assets/072e8414-3281-4b39-a63e-e72a2041b013)
