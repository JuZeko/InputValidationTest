import { test, expect } from "@playwright/test";
import { generateRandomString } from "./helpers/generate-text";
import {
  MinNameLength,
  MinSurnameLength,
  MaxNameLength,
  MinAgeLength,
} from "./Constants/input-data-constants";

test.beforeEach(async ({ page }) => {
  await page.goto("input-validation.html");
});

//#region Title Tests
test("has title", async ({ page }) => {
  await expect(page).toHaveTitle(/Input Validation/);
});
//#endregion

//#region Form Submission Tests
test("should fill form fields and submit successfully", async ({ page }) => {
  await page.locator("#firstname").fill("TestName");
  await page.locator("#surname").fill("TestSurname");
  await page.locator("#age").fill("18");

  await page.locator('input[type="submit"]').click();
  await expect(page).toHaveTitle(/Input Validation/);
});
//#endregion

//#region Form Submission Tests
test("should not submit successfully if surname is empty", async ({ page }) => {
  for (let i = MinNameLength; i <= MaxNameLength; i++) {
    await page.locator("#firstname").fill(generateRandomString(i));
    await page.locator("#surname").fill(generateRandomString(0));
    await page.locator("#age").fill(MinAgeLength.toString());
    await page.locator('input[type="submit"]').click();

    if (await page.isVisible("text='Input Validation Response'")) {
      console.log("Problem with empty surname");
      await page.goBack();
      break;
    }
  }
});

test("should not submit successfully if firstname is less than limit", async ({
  page,
}) => {
  for (let i = MinNameLength; (i) => MinNameLength; i--) {
    await page.locator("#firstname").fill(generateRandomString(i));
    await page.locator("#surname").fill(generateRandomString(MinSurnameLength));
    await page.locator("#age").fill(MinAgeLength.toString());
    await page.locator('input[type="submit"]').click();

    if (await page.isVisible("text='Input Validation Response'")) {
      console.log(`Problem with name length: ${i}`);
      await page.goBack();
      break;
    }
  }
});
//#endregion
