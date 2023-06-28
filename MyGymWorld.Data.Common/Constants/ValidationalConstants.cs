﻿namespace MyGymWorld.Data.Common.Constants
{
    using System.Runtime.InteropServices;

    public static class ValidationalConstants
    {
        public static class CountryConstants
        {
            public const int NameMinLength = 4;
            public const int NameMaxLength = 100;
        }

        public static class TownConstants
        {
            public const int NameMinLength = 1;
            public const int NameMaxLength = 100;

            public const int PopulationMinValue = 50;
            public const int PopulationMaxValue = 50_000_000;

            public const int ZipCodeMinLength = 5;
            public const int ZipCodeMaxLength = 9;
        }

        public static class AddressConstants
        {
            public const int NameMinLength = 5;
            public const int NameMaxLength = 100;
        }

        public static class GymConstants
        {
            public const int NameMinLength = 5;
            public const int NameMaxLength = 100;

            public const int PhoneNumberMinLength = 7;
            public const int PhoneNumberMaxLength = 15;

            public const int DescriptionMinLength = 50;
            public const int DescriptionMaxLength = 5000;
        }

        public static class EventTypeConstants
        {
            public const int NameMinLength = 5;
            public const int NameMaxLength = 15;
        }

        public static class EventConstants
        {
            public const int NameMinLength = 5;
            public const int NameMaxLength = 20;

            public const int DescriptionMinLength = 15;
            public const int DescriptionMaxLength = 150;
        }

        public static class ArticleConstants
        {
            public const int TitleMinLength = 10;
            public const int TitleMaxLength = 50;

            public const int ContentMinLength = 50;
            public const int ContentMaxLength = 5000;
        }

        public static class CategoryConstants
        {
            public const int NameMinLength = 5;
            public const int NameMaxLength = 50;
        }

        public static class CommentConstants
        {
            public const int ContentMinLength = 10;
            public const int ContentMaxLength = 500;
        }
    }
}