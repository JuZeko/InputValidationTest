using Bogus;


namespace InputValidation.InputValidation.Helper
{
    public static class DataGenerationHelper
    {
        private static readonly Faker Faker = new();

        public static string GenerateFirstName(int lenghtOfTheName) => Faker.Lorem.Letter(lenghtOfTheName);

        public static string GenerateSurname(int lenghtOfTheSurname) => Faker.Lorem.Letter(lenghtOfTheSurname);

        public static string GenerateAge(int minAgeLength, int maxAgeLength) => Faker.Random.Int(minAgeLength, maxAgeLength).ToString();
    }
}
