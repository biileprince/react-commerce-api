using ReactCommerce.Api.Entities;

namespace ReactCommerce.Api.Data;

public static class SeedData
{
    public static async Task Initialize(AppDbContext db)
    {
        if (db.Categories.Any()) return;

        // Categories
        var categories = new Dictionary<string, Category>
        {
            ["electronics"] = new() { Id = Guid.NewGuid(), Name = "Electronics", Slug = "electronics", Description = "Phones, laptops, TVs, and electronic accessories", Icon = "smartphone", SortOrder = 1 },
            ["fashion"] = new() { Id = Guid.NewGuid(), Name = "Fashion", Slug = "fashion", Description = "Clothing, shoes, bags, and fashion accessories", Icon = "shopping-bag", SortOrder = 2 },
            ["home"] = new() { Id = Guid.NewGuid(), Name = "Home & Living", Slug = "home", Description = "Furniture, home decor, kitchen, and appliances", Icon = "home", SortOrder = 3 },
            ["beauty"] = new() { Id = Guid.NewGuid(), Name = "Beauty & Health", Slug = "beauty", Description = "Skincare, makeup, fragrances, and health products", Icon = "heart", SortOrder = 4 },
            ["sports"] = new() { Id = Guid.NewGuid(), Name = "Sports & Outdoors", Slug = "sports", Description = "Sporting equipment, fitness gear, and outdoor activities", Icon = "activity", SortOrder = 5 },
            ["toys"] = new() { Id = Guid.NewGuid(), Name = "Toys & Games", Slug = "toys", Description = "Toys, games, and entertainment for kids", Icon = "smile", SortOrder = 6 },
        };

        db.Categories.AddRange(categories.Values);

        // Demo user
        var demoUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Kofi Mensah",
            Email = "kofi@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            Role = "customer",
        };
        db.Users.Add(demoUser);

      
        SeedProduct(db, categories, "Wireless Bluetooth Headphones", "wireless-bluetooth-headphones",
            "Premium wireless headphones with active noise cancellation, 30-hour battery life, and crystal-clear audio. Perfect for music lovers and remote workers alike. Features touch controls, built-in microphone, and foldable design for easy portability.",
            450, "electronics", 50, 4.8m, 234, true,
            ["https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=1200&q=80",
             "https://images.unsplash.com/photo-1546435770-a3e426bf472b?auto=format&fit=crop&w=1200&q=80"],
            ["headphones", "wireless", "noise-cancelling", "bluetooth"]);

        SeedProduct(db, categories, "Smart Fitness Watch", "smart-fitness-watch",
            "Advanced smartwatch with continuous heart rate tracking, built-in GPS, sleep analysis, and 7-day battery life. Compatible with iOS and Android. Water-resistant up to 50 meters. Tracks 100+ workout modes.",
            350, "electronics", 30, 4.6m, 189, true,
            ["https://images.unsplash.com/photo-1523275335684-37898b6baf30?auto=format&fit=crop&w=1200&q=80",
             "https://images.unsplash.com/photo-1579586337278-3befd40fd17a?auto=format&fit=crop&w=1200&q=80"],
            ["smartwatch", "fitness", "GPS", "health"]);

        SeedProduct(db, categories, "Portable Power Bank 20000mAh", "portable-power-bank-20000mah",
            "High-capacity power bank with fast charging technology (22.5W), dual USB-A ports, and USB-C PD port. LED battery indicator. Charges your phone up to 5 times on a single charge. Airline-approved.",
            120, "electronics", 100, 4.5m, 312, false,
            ["https://images.unsplash.com/photo-1609091839311-d5365f9ff1c5?auto=format&fit=crop&w=1200&q=80",
             "https://images.unsplash.com/photo-1583863788434-e58a36330cf0?auto=format&fit=crop&w=1200&q=80"],
            ["power-bank", "charging", "portable"]);

        SeedProduct(db, categories, "Wireless Gaming Mouse", "wireless-gaming-mouse",
            "High-precision wireless gaming mouse with 25,600 DPI optical sensor, 70-hour battery life, 6 programmable buttons, and customizable RGB lighting. Ultra-lightweight at 95g for competitive gaming.",
            180, "electronics", 40, 4.7m, 156, false,
            ["https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?auto=format&fit=crop&w=1200&q=80",
             "https://images.unsplash.com/photo-1629429407756-01cd3d7cfb38?auto=format&fit=crop&w=1200&q=80"],
            ["gaming", "mouse", "wireless", "RGB"]);

        SeedProduct(db, categories, "USB-C Hub 7-in-1", "usb-c-hub-7-in-1",
            "Versatile USB-C hub expanding your laptop connectivity with HDMI 4K output, 3x USB 3.0 ports, SD & microSD card readers, and 100W Power Delivery passthrough. Plug-and-play, no drivers needed.",
            135, "electronics", 60, 4.4m, 98, false,
            ["https://images.unsplash.com/photo-1587829741301-dc798b83add3?auto=format&fit=crop&w=1200&q=80"],
            ["USB-C", "hub", "adapter", "laptop"]);

        SeedProduct(db, categories, "Mechanical Keyboard RGB", "mechanical-keyboard-rgb",
            "Tactile mechanical keyboard with Cherry MX Brown switches, full RGB per-key backlighting, aluminum top frame, and N-key rollover. Compact tenkeyless design saves desk space without sacrificing function.",
            280, "electronics", 25, 4.9m, 445, true,
            ["https://images.unsplash.com/photo-1587829741301-dc798b83add3?auto=format&fit=crop&w=1200&q=80",
             "https://images.unsplash.com/photo-1541140532154-b024d705b90a?auto=format&fit=crop&w=1200&q=80"],
            ["keyboard", "mechanical", "RGB", "gaming"]);

        SeedProduct(db, categories, "True Wireless Earbuds Pro", "true-wireless-earbuds-pro",
            "Premium true wireless earbuds with active noise cancellation, 28-hour total battery (7h + 21h case), IPX5 water resistance, multipoint connection, and custom-tuned 10mm drivers for exceptional bass.",
            320, "electronics", 45, 4.6m, 278, false,
            ["https://images.unsplash.com/photo-1590658268037-6bf12165a8df?auto=format&fit=crop&w=1200&q=80"],
            ["earbuds", "wireless", "ANC", "audio"]);

        SeedProduct(db, categories, "4K Action Camera", "4k-action-camera",
            "Rugged 4K/60fps action camera with electronic image stabilization, 170 degree wide angle, waterproof to 30m without housing, voice control, and built-in Wi-Fi. Includes 2 rechargeable batteries.",
            650, "electronics", 15, 4.7m, 134, true,
            ["https://images.unsplash.com/photo-1502920917128-1aa500764cbd?auto=format&fit=crop&w=1200&q=80"],
            ["camera", "4K", "action", "waterproof"]);

        // Fashion
        SeedProduct(db, categories, "Laptop Backpack — Water Resistant", "laptop-backpack-water-resistant",
            "Durable 30L backpack with dedicated padded laptop compartment (fits up to 17\"), multiple organization pockets, USB charging port, ergonomic air-mesh back panel, and water-resistant 900D polyester exterior.",
            260, "fashion", 75, 4.5m, 203, false,
            ["https://images.unsplash.com/photo-1553062407-98eeb64c6a62?auto=format&fit=crop&w=1200&q=80",
             "https://images.unsplash.com/photo-1581605405669-fcdf81165afa?auto=format&fit=crop&w=1200&q=80"],
            ["backpack", "laptop", "travel", "bag"]);

        SeedProduct(db, categories, "Slim Fit Chino Trousers", "slim-fit-chino-trousers",
            "Classic slim-fit chinos crafted from stretch cotton blend for all-day comfort. Available in multiple colors.",
            195, "fashion", 80, 4.3m, 167, false,
            ["https://images.unsplash.com/photo-1624378439575-d8705ad7ae80?auto=format&fit=crop&w=1200&q=80"],
            ["chinos", "trousers", "slim-fit", "men"]);

        SeedProduct(db, categories, "Leather Crossbody Bag", "leather-crossbody-bag",
            "Genuine full-grain leather crossbody bag with adjustable strap, gold-tone hardware, and suede lining.",
            380, "fashion", 35, 4.8m, 92, true,
            ["https://images.unsplash.com/photo-1548036328-c9fa89d128fa?auto=format&fit=crop&w=1200&q=80"],
            ["bag", "leather", "crossbody", "women"]);

        SeedProduct(db, categories, "Classic White Sneakers", "classic-white-sneakers",
            "Iconic minimalist white canvas sneakers with vulcanized rubber sole and cushioned insole. Unisex design.",
            220, "fashion", 60, 4.6m, 328, false,
            ["https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=1200&q=80"],
            ["sneakers", "shoes", "white", "unisex"]);

        // Home
        SeedProduct(db, categories, "Ceramic Pour-Over Coffee Set", "ceramic-pour-over-coffee-set",
            "Handcrafted ceramic pour-over coffee dripper with matching carafe and two mugs. Gift-boxed and dishwasher safe.",
            175, "home", 40, 4.7m, 85, false,
            ["https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?auto=format&fit=crop&w=1200&q=80"],
            ["coffee", "ceramic", "pour-over", "kitchen"]);

        SeedProduct(db, categories, "Bamboo Desk Organizer", "bamboo-desk-organizer",
            "Eco-friendly bamboo desk organizer with 5 compartments for pens, scissors, phones, and stationery.",
            85, "home", 55, 4.4m, 143, false,
            ["https://images.unsplash.com/photo-1518455027359-f3f8164ba6bd?auto=format&fit=crop&w=1200&q=80"],
            ["desk", "organizer", "bamboo", "office"]);

        SeedProduct(db, categories, "Scented Soy Candle Set", "scented-soy-candle-set",
            "Set of 3 hand-poured 100% soy wax candles in premium glass vessels. 45-hour burn time each.",
            145, "home", 70, 4.9m, 211, false,
            ["https://images.unsplash.com/photo-1571781926291-c477ebfd024b?auto=format&fit=crop&w=1200&q=80"],
            ["candle", "soy", "scented", "home-decor"]);

        SeedProduct(db, categories, "Premium Bed Linen Set", "premium-bed-linen-set",
            "400-thread-count Egyptian cotton sateen bed linen set including duvet cover, 2 pillowcases, and flat sheet.",
            480, "home", 20, 4.8m, 76, false,
            ["https://images.unsplash.com/photo-1522771739844-6a9f6d5f14af?auto=format&fit=crop&w=1200&q=80"],
            ["bedding", "linen", "cotton", "bedroom"]);

        // Beauty
        SeedProduct(db, categories, "Vitamin C Brightening Serum", "vitamin-c-brightening-serum",
            "15% L-Ascorbic Acid serum with hyaluronic acid and vitamin E. Dermatologist-tested, fragrance-free. 30ml.",
            95, "beauty", 90, 4.7m, 398, false,
            ["https://images.unsplash.com/photo-1620916566398-39f1143ab7be?auto=format&fit=crop&w=1200&q=80"],
            ["serum", "vitamin-c", "skincare", "brightening"]);

        SeedProduct(db, categories, "Professional Hair Dryer", "professional-hair-dryer",
            "Ionic 2400W professional hair dryer with 3 heat settings, 2 speed settings, cool shot button. Folds for travel.",
            210, "beauty", 35, 4.5m, 162, false,
            ["https://images.unsplash.com/photo-1522338242992-e1a54906a8da?auto=format&fit=crop&w=1200&q=80"],
            ["hair", "dryer", "ionic", "beauty"]);

        SeedProduct(db, categories, "Natural Beard Grooming Kit", "natural-beard-grooming-kit",
            "Complete beard care set including beard oil (jojoba + argan), beard balm, brush, comb, and scissors.",
            130, "beauty", 50, 4.6m, 88, false,
            ["https://images.unsplash.com/photo-1621607512214-68297480165e?auto=format&fit=crop&w=1200&q=80"],
            ["beard", "grooming", "men", "skincare"]);

        // Sports
        SeedProduct(db, categories, "Adjustable Dumbbell Set", "adjustable-dumbbell-set",
            "Space-saving adjustable dumbbell set. Adjusts from 2kg to 24kg in 2kg increments per dumbbell. Includes storage tray.",
            890, "sports", 18, 4.8m, 124, true,
            ["https://images.unsplash.com/photo-1526506118085-60ce8714f8c5?auto=format&fit=crop&w=1200&q=80"],
            ["dumbbell", "fitness", "gym", "weights"]);

        SeedProduct(db, categories, "Yoga Mat Premium Non-Slip", "yoga-mat-premium-non-slip",
            "6mm thick TPE eco-friendly yoga mat with alignment lines, double-sided non-slip texture. Includes carry strap.",
            115, "sports", 65, 4.5m, 287, false,
            ["https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?auto=format&fit=crop&w=1200&q=80"],
            ["yoga", "mat", "fitness", "exercise"]);

        SeedProduct(db, categories, "Hiking Boots Waterproof", "hiking-boots-waterproof",
            "Full-grain leather hiking boots with Gore-Tex waterproof membrane, Vibram outsole for superior grip.",
            420, "sports", 28, 4.7m, 95, false,
            ["https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=1200&q=80"],
            ["hiking", "boots", "waterproof", "outdoor"]);

        // Toys
        SeedProduct(db, categories, "STEM Robot Building Kit", "stem-robot-building-kit",
            "Educational robotics kit for ages 8+. Build 5 different robot models with 190+ components. No soldering required.",
            240, "toys", 30, 4.9m, 73, false,
            ["https://images.unsplash.com/photo-1561144257-e32e8efc6c4f?auto=format&fit=crop&w=1200&q=80"],
            ["STEM", "robot", "educational", "kids"]);

        SeedProduct(db, categories, "Strategy Board Game", "strategy-board-game",
            "Award-winning strategy board game for 2-6 players, ages 10+. Premium wooden pieces and replayable scenarios.",
            160, "toys", 42, 4.7m, 118, false,
            ["https://images.unsplash.com/photo-1610890716171-6b1bb98ffd09?auto=format&fit=crop&w=1200&q=80"],
            ["board-game", "strategy", "family", "games"]);

        await db.SaveChangesAsync();
    }

    private static void SeedProduct(
        AppDbContext db, Dictionary<string, Category> categories,
        string name, string slug, string description,
        decimal price, string categorySlug, int stock,
        decimal rating, int reviewCount, bool isFeatured,
        string[] imageUrls, string[] tags)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug,
            Description = description,
            Price = price,
            StockQuantity = stock,
            CategoryId = categories[categorySlug].Id,
            Rating = rating,
            ReviewCount = reviewCount,
            IsFeatured = isFeatured,
        };

        db.Products.Add(product);

        for (var i = 0; i < imageUrls.Length; i++)
        {
            db.ProductImages.Add(new ProductImage
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Url = imageUrls[i],
                SortOrder = i,
            });
        }

        foreach (var tag in tags)
        {
            db.ProductTags.Add(new ProductTag
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Tag = tag.ToLower(),
            });
        }
    }
}
