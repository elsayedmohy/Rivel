namespace RiverLine.Api.Data.Seeds;

public static class NileBerthSeed
{
    public static void Seed(ModelBuilder builder)
    {
        builder.Entity<NileBerth>().HasData(
            // Cairo–Damietta Axis
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Damietta River Port",
                ArabicName = "ميناء دمياط النهري",
                Governorate = "دمياط",
                Latitude = 31.460344m,
                Longitude = 31.756536m,
                Type = BerthType.Port,
                Axis = NavigationAxis.CairoDamietta,
                CoordinateAccuracy = CoordinateAccuracy.Exact, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "Talkha Pier",
                ArabicName = "رصيف طلخا",
                Governorate = "الدقهلية",
                Latitude = 31.059511m,
                Longitude = 31.398000m,
                Type = BerthType.Pier,
                Axis = NavigationAxis.CairoDamietta,
                CoordinateAccuracy = CoordinateAccuracy.Exact,
                IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "Talkha Pier 2",
                ArabicName = "رصيف طلخا 2",
                Governorate = "الدقهلية",
                Latitude = 31.048407m,
                Longitude = 31.357553m,
                Type = BerthType.Pier,
                Axis = NavigationAxis.CairoDamietta,
                CoordinateAccuracy = CoordinateAccuracy.Exact,
                IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                Name = "Zifta River Dock",
                ArabicName = "رصيف زفتى النهري",
                Governorate = "الغربية",
                Latitude = 30.710902m,
                Longitude = 31.255163m,
                Type = BerthType.Dock,
                Axis = NavigationAxis.CairoDamietta,
                CoordinateAccuracy = CoordinateAccuracy.Exact,
                IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Name = "Delta Barrage",
                ArabicName = "القناطر الخيرية", Governorate = "القليوبية", Latitude = 30.205m, Longitude = 31.126m,
                Type = BerthType.LandingSite, Axis = NavigationAxis.CairoDamietta,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), Name = "Banha River Port",
                ArabicName = "ميناء بنها النهري", Governorate = "القليوبية", Latitude = 30.466m, Longitude = 31.185m,
                Type = BerthType.Port, Axis = NavigationAxis.CairoDamietta,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },

            // Cairo–Aswan Axis
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000007"), Name = "Athar El Nabi",
                ArabicName = "آثار النبي", Governorate = "القاهرة", Latitude = 30.115057m, Longitude = 31.215250m,
                Type = BerthType.Dock, Axis = NavigationAxis.CairoAswan, CoordinateAccuracy = CoordinateAccuracy.Exact,
                IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000008"), Name = "Imbaba Tankers",
                ArabicName = "إمبابة الناقلات", Governorate = "الجيزة", Latitude = 30.062m, Longitude = 31.207m,
                Type = BerthType.Terminal, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000009"), Name = "Imbaba River Port",
                ArabicName = "ميناء إمبابة النهري", Governorate = "الجيزة", Latitude = 30.076m, Longitude = 31.208m,
                Type = BerthType.Port, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000010"), Name = "El Tebbin Limestone",
                ArabicName = "التبين - حجر جيري", Governorate = "القاهرة", Latitude = 29.805m, Longitude = 31.331m,
                Type = BerthType.Terminal, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000011"), Name = "El Tebbin Coke",
                ArabicName = "التبين - كوك", Governorate = "القاهرة", Latitude = 29.798m, Longitude = 31.320m,
                Type = BerthType.Terminal, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), Name = "El Tebbin El Nahree",
                ArabicName = "التبين النهري", Governorate = "القاهرة", Latitude = 29.800m, Longitude = 31.325m,
                Type = BerthType.Dock, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000013"), Name = "Tora Cement",
                ArabicName = "طرة - أسمنت", Governorate = "القاهرة", Latitude = 29.927m, Longitude = 31.281m,
                Type = BerthType.Terminal, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000014"), Name = "El Masara", ArabicName = "المسارة",
                Governorate = "القاهرة", Latitude = 29.906m, Longitude = 31.282m, Type = BerthType.Dock,
                Axis = NavigationAxis.CairoAswan, CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000015"), Name = "El Kawmiya Cement",
                ArabicName = "القومية - أسمنت", Governorate = "بني سويف", Latitude = 29.95m, Longitude = 31.20m,
                Type = BerthType.Terminal, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000016"), Name = "Samalout Cement",
                ArabicName = "سمالوط - أسمنت", Governorate = "المنيا", Latitude = 28.312m, Longitude = 30.711m,
                Type = BerthType.Terminal, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000017"), Name = "Samalout River Node",
                ArabicName = "سمالوط - عقدة نهرية", Governorate = "المنيا", Latitude = 28.312m, Longitude = 30.711m,
                Type = BerthType.Dock, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000018"), Name = "Minya River Port",
                ArabicName = "ميناء المنيا النهري", Governorate = "المنيا", Latitude = 28.109m, Longitude = 30.750m,
                Type = BerthType.Port, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000019"), Name = "Bany Khaled / Samalout",
                ArabicName = "بني خالد / سمالوط", Governorate = "المنيا", Latitude = 28.420m, Longitude = 30.760m,
                Type = BerthType.Dock, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000020"), Name = "Minya Logistics River Port",
                ArabicName = "ميناء المنيا اللوجستي", Governorate = "المنيا", Latitude = 28.110m, Longitude = 30.745m,
                Type = BerthType.Port, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000021"), Name = "El Akaba", ArabicName = "العقبة",
                Governorate = "أسيوط", Latitude = 27.22m, Longitude = 31.18m, Type = BerthType.Dock,
                Axis = NavigationAxis.CairoAswan, CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000022"), Name = "Asyut Calories Station",
                ArabicName = "محطة أسيوط - حبوب", Governorate = "أسيوط", Latitude = 27.180m, Longitude = 31.185m,
                Type = BerthType.Terminal, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000023"), Name = "Asyut Petrol Port",
                ArabicName = "ميناء أسيوط البترولي", Governorate = "أسيوط", Latitude = 27.18m, Longitude = 31.19m,
                Type = BerthType.Port, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000024"), Name = "Asyut Cement – Menkbad",
                ArabicName = "أسيوط أسمنت - منقباد", Governorate = "أسيوط", Latitude = 27.215m, Longitude = 31.190m,
                Type = BerthType.Terminal, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000025"), Name = "Menkbad Fertilizer Port",
                ArabicName = "ميناء منقباد - أسمدة", Governorate = "أسيوط", Latitude = 27.218m, Longitude = 31.185m,
                Type = BerthType.Port, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000026"), Name = "Asyut River Port",
                ArabicName = "ميناء أسيوط النهري", Governorate = "أسيوط", Latitude = 27.180m, Longitude = 31.183m,
                Type = BerthType.Port, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000027"), Name = "Sohag River Port",
                ArabicName = "ميناء سوهاج النهري", Governorate = "سوهاج", Latitude = 26.56m, Longitude = 31.70m,
                Type = BerthType.Port, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000028"), Name = "Gerga Sugar",
                ArabicName = "جرجا - سكر", Governorate = "سوهاج", Latitude = 26.34m, Longitude = 31.89m,
                Type = BerthType.Terminal, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000029"), Name = "El Balina", ArabicName = "البلينا",
                Governorate = "سوهاج", Latitude = 26.25m, Longitude = 32.00m, Type = BerthType.Dock,
                Axis = NavigationAxis.CairoAswan, CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000030"), Name = "River Aluminum",
                ArabicName = "ألومنيوم النهر", Governorate = "قنا", Latitude = 26.05m, Longitude = 32.72m,
                Type = BerthType.Terminal, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000031"), Name = "Nagaa Hammady Sugar",
                ArabicName = "نجع حمادي - سكر", Governorate = "قنا", Latitude = 26.04m, Longitude = 32.24m,
                Type = BerthType.Terminal, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000032"), Name = "Dishna Sugar",
                ArabicName = "دشنا - سكر", Governorate = "قنا", Latitude = 26.09m, Longitude = 32.58m,
                Type = BerthType.Terminal, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000033"), Name = "Koss Sugar", ArabicName = "قوص - سكر",
                Governorate = "قنا", Latitude = 25.91m, Longitude = 32.76m, Type = BerthType.Terminal,
                Axis = NavigationAxis.CairoAswan, CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000034"), Name = "Dandara River Port",
                ArabicName = "ميناء دندرة النهري", Governorate = "قنا", Latitude = 26.14m, Longitude = 32.66m,
                Type = BerthType.Port, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000035"), Name = "Qena River Port",
                ArabicName = "ميناء قنا النهري", Governorate = "قنا", Latitude = 26.16m, Longitude = 32.72m,
                Type = BerthType.Port, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000036"), Name = "Luxor River Port",
                ArabicName = "ميناء الأقصر النهري", Governorate = "الأقصر", Latitude = 25.69m, Longitude = 32.64m,
                Type = BerthType.Port, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000037"), Name = "Armant Sugar",
                ArabicName = "أرمنت - سكر", Governorate = "الأقصر", Latitude = 25.62m, Longitude = 32.55m,
                Type = BerthType.Terminal, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000038"), Name = "Esna River Port",
                ArabicName = "ميناء إسنا النهري", Governorate = "الأقصر", Latitude = 25.29m, Longitude = 32.55m,
                Type = BerthType.Port, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000039"), Name = "El Sibaaya", ArabicName = "السباعية",
                Governorate = "الأقصر", Latitude = 25.52m, Longitude = 32.74m, Type = BerthType.Terminal,
                Axis = NavigationAxis.CairoAswan, CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000040"), Name = "Edfu Sugar", ArabicName = "إدفو - سكر",
                Governorate = "أسوان", Latitude = 24.98m, Longitude = 32.88m, Type = BerthType.Terminal,
                Axis = NavigationAxis.CairoAswan, CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000041"), Name = "Edfu River Dock",
                ArabicName = "رصيف إدفو النهري", Governorate = "أسوان", Latitude = 24.98m, Longitude = 32.88m,
                Type = BerthType.Dock, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000042"), Name = "Aswan River Port / El Akab",
                ArabicName = "ميناء أسوان / العقب", Governorate = "أسوان", Latitude = 24.10m, Longitude = 32.90m,
                Type = BerthType.Port, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000043"), Name = "Aswan River Port",
                ArabicName = "ميناء أسوان النهري", Governorate = "أسوان", Latitude = 24.09m, Longitude = 32.90m,
                Type = BerthType.Port, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000047"), Name = "Firo-Silicon Factory",
                ArabicName = "مصنع فيرو سيليكون", Governorate = "أسوان", Latitude = 24.15m, Longitude = 32.90m,
                Type = BerthType.Terminal, Axis = NavigationAxis.CairoAswan,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },

            // Aswan–Wadi Halfa Axis
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000044"), Name = "Abu Simbel Pier",
                ArabicName = "رصيف أبو سمبل", Governorate = "أسوان", Latitude = 22.34m, Longitude = 31.62m,
                Type = BerthType.Pier, Axis = NavigationAxis.AswanWadiHalfa,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000045"), Name = "Tushka / Amada",
                ArabicName = "توشكى / عمدة", Governorate = "أسوان", Latitude = 22.80m, Longitude = 31.20m,
                Type = BerthType.LandingSite, Axis = NavigationAxis.AswanWadiHalfa,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000046"), Name = "El Hadid & El Solb",
                ArabicName = "الحديد والصلب", Governorate = "أسوان", Latitude = 24.08m, Longitude = 32.89m,
                Type = BerthType.Terminal, Axis = NavigationAxis.AswanWadiHalfa,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            },
            new NileBerth
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000048"), Name = "Lake Nasser / Aswan Landing",
                ArabicName = "بحيرة ناصر / إنزال أسوان", Governorate = "أسوان", Latitude = 24.09m, Longitude = 32.90m,
                Type = BerthType.LandingSite, Axis = NavigationAxis.AswanWadiHalfa,
                CoordinateAccuracy = CoordinateAccuracy.Approximate, IsActive = true
            }
        );
    }
}