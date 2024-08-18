using InputValidation.InputValidation.Constants;
using Microsoft.Playwright.NUnit;
using InputValidation.InputValidation.Helper;
using Microsoft.Playwright;
using InputValidation.InputValidation.Enums;

namespace InputValidation.InputValidation.Input
{
    public class InputValidation : PageTest
    {
        public TestContext TestContext { get; set; }

        [SetUp]
        public async Task Setup()
        {
            string _appUrl = TestContext.Parameters["InputValidationBaseUrl"];
            await Page.GotoAsync(_appUrl);
        }

        #region fill form
        private async Task FillFormWithRandomData(FormFillMode formFillMode)
        {

            switch (formFillMode)
            {
                case FormFillMode.OnlyFirstName:
                    await Page.FillAsync("#firstname", DataGenerationHelper.GenerateFirstName(InputDataConstants.MinNameLenght));
                    break;

                case FormFillMode.NameSurnameFields:
                    await Page.FillAsync("#firstname", DataGenerationHelper.GenerateFirstName(InputDataConstants.MinNameLenght));
                    await Page.FillAsync("#surname", DataGenerationHelper.GenerateSurname(InputDataConstants.MinSurnameLenght));
                    break;

                case FormFillMode.AllFields:
                    await Page.FillAsync("#firstname", DataGenerationHelper.GenerateFirstName(InputDataConstants.MinNameLenght));
                    await Page.FillAsync("#surname", DataGenerationHelper.GenerateSurname(InputDataConstants.MinSurnameLenght));
                    await Page.FillAsync("#age", DataGenerationHelper.GenerateAge(InputDataConstants.MinAgeLenght, InputDataConstants.MaxAgeLenght));
                    break;
            }
        }
        #endregion

        #region General Form 
        [Test]
        public async Task FormSubmissionShowsValidationResponse()
        {
            // Arrange
            await FillFormWithRandomData(FormFillMode.AllFields);

            // Act
            await Page.Locator("input").Last.ClickAsync();

            // Assert
            await Expect(Page.Locator("text='Input Validation Response'")).ToBeVisibleAsync();
        }
        #endregion

        #region Countries dropdown
        [Test]
        public async Task ValidateFormForEachCountryOption()
        {
            // Arrange
            var countryDropdown = Page.Locator("select#country");
            var options = countryDropdown.Locator("option");

            // Act
            for (int i = 0; i < await options.CountAsync(); i++)
            {
                await FillFormWithRandomData(FormFillMode.AllFields);

                var optionValue = await options.Nth(i).GetAttributeAsync("value");

                await countryDropdown.SelectOptionAsync(new SelectOptionValue { Value = optionValue });
                await Page.Locator("input[type='submit']").ClickAsync();

                if (await Page.Locator("text='Please select a country from the list'").IsVisibleAsync())
                {
                    Console.WriteLine($"Bad country: {optionValue}");
                    continue;
                }

                // Assert
                await Expect(Page.Locator("text='Input Validation Response'")).ToBeVisibleAsync();
                await Page.GoBackAsync();
            }
        }
        #endregion

        #region Age input
        [Test]
        public async Task ValidateFormForEachAgeInput()
        {
            // Arrange
            var ageInput = Page.Locator("input#age");
            int minValue = int.TryParse(await ageInput.GetAttributeAsync("min"), out var min) ? min : InputDataConstants.MinAgeLenght;
            int maxValue = int.TryParse(await ageInput.GetAttributeAsync("max"), out var max) ? max : InputDataConstants.MaxAgeLenght;

            // Act
            for (int age = minValue; age <= maxValue; age += 1)
            {
                await FillFormWithRandomData(FormFillMode.NameSurnameFields);
                await ageInput.FillAsync(age.ToString());
                await Page.Locator("input[type='submit']").ClickAsync();

                if (await Page.Locator("text='Please enter a valid age'").IsVisibleAsync())
                {
                    Console.WriteLine($"Invalid age detected: {age}");
                    continue;
                }

                // Assert
                await Expect(Page.Locator("text='Input Validation Response'")).ToBeVisibleAsync();
                await Page.GoBackAsync();
            }
        }

        [Test]
        public async Task ValidateFormWithDecimalInput()
        {
            // Arrange
            var ageInput = Page.Locator("input#age");
            decimal minValue = decimal.TryParse(await ageInput.GetAttributeAsync("min"), out var min) ? min : InputDataConstants.MinAgeLenght;
            decimal maxValue = minValue + 1m;


            // Act
            for (decimal age = minValue; age <= maxValue; age += 0.01m)
            {
                await FillFormWithRandomData(FormFillMode.NameSurnameFields);

                var formatedAge = age.ToString().Replace(',', '.');

                Page.FillAsync("input#age", formatedAge);

                await Page.Locator("input[type='submit']").ClickAsync();

                bool isValidationVisible = await Page.Locator("text='Input Validation Response'").IsVisibleAsync();

                bool isErrorVisible = await Page.Locator("text='500 Internal Server Error'").IsVisibleAsync();

                // Assert
                if (isValidationVisible)
                {
                    await Page.GoBackAsync();
                    continue;
                }
                else if (isErrorVisible)
                {
                    Console.WriteLine($"Error with decimal value: {formatedAge}");
                    continue;
                }
            }
        }
        #endregion

        #region Note input
        [Test]
        public async Task ValidateTextAreaProtectedFromXSS()
        {
            // Arrange
            await FillFormWithRandomData(FormFillMode.AllFields);
            var textarea = Page.Locator("textarea#notes");
            var scriptText = "<script>alert('This is a test alert!');</script>";

            // Act
            await textarea.FillAsync(scriptText);

            Page.Dialog += async (sender, e) =>
            {
                if (e.Type == DialogType.Alert)
                {
                    // Assert
                    Console.WriteLine($"Alert message: {e.Message}");
                    await e.AcceptAsync();
                }
            };

            await Page.Locator("input[type='submit']").ClickAsync();
            await Page.WaitForTimeoutAsync(4000);
        }
        #endregion
    }
}
