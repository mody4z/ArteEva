-- SQL Script to Insert 100 Products into ArtEva Database
-- Run this script on the ArtEva database

USE ArtEva;
GO

-- Variables to store IDs
DECLARE @ShopId INT;
DECLARE @CategoryId1 INT, @CategoryId2 INT, @CategoryId3 INT, @CategoryId4 INT, @CategoryId5 INT;
DECLARE @SubCatId1 INT, @SubCatId2 INT, @SubCatId3 INT, @SubCatId4 INT, @SubCatId5 INT;
DECLARE @Counter INT = 1;
DECLARE @CurrentDate DATETIME = GETDATE();

-- Get first active shop
SELECT TOP 1 @ShopId = Id FROM Shops WHERE IsDeleted = 0 AND Status = 0; -- Status 0 = Active

-- Get category IDs
SELECT TOP 1 @CategoryId1 = Id FROM Categories WHERE IsDeleted = 0 ORDER BY Id;
SELECT @CategoryId2 = Id FROM Categories WHERE IsDeleted = 0 AND Id > @CategoryId1 ORDER BY Id OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @CategoryId3 = Id FROM Categories WHERE IsDeleted = 0 AND Id > @CategoryId2 ORDER BY Id OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @CategoryId4 = Id FROM Categories WHERE IsDeleted = 0 AND Id > @CategoryId3 ORDER BY Id OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @CategoryId5 = Id FROM Categories WHERE IsDeleted = 0 AND Id > @CategoryId4 ORDER BY Id OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;

-- Get subcategory IDs
SELECT TOP 1 @SubCatId1 = Id FROM SubCategories WHERE IsDeleted = 0 AND CategoryId = @CategoryId1;
SELECT TOP 1 @SubCatId2 = Id FROM SubCategories WHERE IsDeleted = 0 AND CategoryId = @CategoryId2;
SELECT TOP 1 @SubCatId3 = Id FROM SubCategories WHERE IsDeleted = 0 AND CategoryId = @CategoryId3;
SELECT TOP 1 @SubCatId4 = Id FROM SubCategories WHERE IsDeleted = 0 AND CategoryId = @CategoryId4;
SELECT TOP 1 @SubCatId5 = Id FROM SubCategories WHERE IsDeleted = 0 AND CategoryId = @CategoryId5;

-- Print verification
PRINT 'Shop ID: ' + CAST(@ShopId AS VARCHAR(10));
PRINT 'Category IDs: ' + CAST(@CategoryId1 AS VARCHAR(10)) + ', ' + CAST(@CategoryId2 AS VARCHAR(10)) + ', ' + 
      CAST(@CategoryId3 AS VARCHAR(10)) + ', ' + CAST(@CategoryId4 AS VARCHAR(10)) + ', ' + CAST(@CategoryId5 AS VARCHAR(10));

-- Check if we have valid IDs
IF @ShopId IS NULL
BEGIN
    PRINT 'ERROR: No active shop found. Please create a shop first.';
    RETURN;
END

IF @CategoryId1 IS NULL OR @SubCatId1 IS NULL
BEGIN
    PRINT 'ERROR: No categories or subcategories found. Please run data seeder first.';
    RETURN;
END

-- Insert 100 Products
PRINT 'Starting to insert 100 products...';

-- Products 1-20: Category 1
INSERT INTO Products (Title, SKU, Price, Status, ApprovalStatus, IsPublished, ShopId, CategoryId, SubCategoryId, CreatedAt, UpdatedAt, IsDeleted)
VALUES
('Abstract Canvas Art #1', 'ART-001', 149.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Modern Sculpture Piece #2', 'ART-002', 299.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Vintage Portrait #3', 'ART-003', 199.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Landscape Oil Painting #4', 'ART-004', 349.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Contemporary Wall Art #5', 'ART-005', 129.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Minimalist Design #6', 'ART-006', 89.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Watercolor Collection #7', 'ART-007', 179.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Acrylic Masterpiece #8', 'ART-008', 259.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Pop Art Print #9', 'ART-009', 99.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Geometric Pattern #10', 'ART-010', 119.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Nature Scene #11', 'ART-011', 189.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Urban Artwork #12', 'ART-012', 159.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Impressionist Style #13', 'ART-013', 399.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Surreal Vision #14', 'ART-014', 279.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Classic Portrait #15', 'ART-015', 449.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Digital Art Print #16', 'ART-016', 79.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Mixed Media Art #17', 'ART-017', 229.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Botanical Illustration #18', 'ART-018', 139.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Coastal Scene #19', 'ART-019', 299.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Mountain Vista #20', 'ART-020', 319.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0);

PRINT 'Inserted products 1-20';

-- Products 21-40: Category 2
INSERT INTO Products (Title, SKU, Price, Status, ApprovalStatus, IsPublished, ShopId, CategoryId, SubCategoryId, CreatedAt, UpdatedAt, IsDeleted)
VALUES
('Bronze Sculpture #21', 'SCU-021', 599.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Ceramic Vase #22', 'SCU-022', 149.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Glass Art Piece #23', 'SCU-023', 199.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Wood Carving #24', 'SCU-024', 249.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Metal Sculpture #25', 'SCU-025', 799.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Abstract Form #26', 'SCU-026', 349.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Stone Statue #27', 'SCU-027', 899.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Polymer Clay Art #28', 'SCU-028', 89.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Wire Sculpture #29', 'SCU-029', 179.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Resin Art #30', 'SCU-030', 129.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Marble Figurine #31', 'SCU-031', 449.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Plaster Cast #32', 'SCU-032', 199.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Contemporary Sculpture #33', 'SCU-033', 549.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Garden Ornament #34', 'SCU-034', 299.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Kinetic Sculpture #35', 'SCU-035', 699.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Terracotta Figure #36', 'SCU-036', 159.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Steel Art #37', 'SCU-037', 849.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Paper Mache #38', 'SCU-038', 79.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Ice Sculpture Photo #39', 'SCU-039', 399.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Light Installation #40', 'SCU-040', 1299.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId2, @CategoryId1), ISNULL(@SubCatId2, @SubCatId1), @CurrentDate, @CurrentDate, 0);

PRINT 'Inserted products 21-40';

-- Products 41-60: Category 3
INSERT INTO Products (Title, SKU, Price, Status, ApprovalStatus, IsPublished, ShopId, CategoryId, SubCategoryId, CreatedAt, UpdatedAt, IsDeleted)
VALUES
('Digital Portrait #41', 'DIG-041', 59.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('3D Render Art #42', 'DIG-042', 99.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Vector Illustration #43', 'DIG-043', 49.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('NFT Artwork #44', 'DIG-044', 499.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Pixel Art Collection #45', 'DIG-045', 29.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Cyberpunk Scene #46', 'DIG-046', 149.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Fantasy Landscape #47', 'DIG-047', 179.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Character Design #48', 'DIG-048', 89.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Concept Art #49', 'DIG-049', 199.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Motion Graphics Print #50', 'DIG-050', 129.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Generative Art #51', 'DIG-051', 349.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Glitch Art #52', 'DIG-052', 69.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Vaporwave Aesthetic #53', 'DIG-053', 79.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Surreal Digital #54', 'DIG-054', 159.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('AI Generated Art #55', 'DIG-055', 99.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Low Poly Design #56', 'DIG-056', 119.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Isometric Art #57', 'DIG-057', 89.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Minimalist Digital #58', 'DIG-058', 49.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Retro Gaming Art #59', 'DIG-059', 59.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Abstract Digital #60', 'DIG-060', 139.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId3, @CategoryId1), ISNULL(@SubCatId3, @SubCatId1), @CurrentDate, @CurrentDate, 0);

PRINT 'Inserted products 41-60';

-- Products 61-80: Category 4
INSERT INTO Products (Title, SKU, Price, Status, ApprovalStatus, IsPublished, ShopId, CategoryId, SubCategoryId, CreatedAt, UpdatedAt, IsDeleted)
VALUES
('Landscape Photography #61', 'PHO-061', 249.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Portrait Series #62', 'PHO-062', 299.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Wildlife Shot #63', 'PHO-063', 349.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Architectural Photo #64', 'PHO-064', 199.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Street Photography #65', 'PHO-065', 179.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Macro Photography #66', 'PHO-066', 159.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Black & White #67', 'PHO-067', 229.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Sunset Collection #68', 'PHO-068', 189.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Urban Exploration #69', 'PHO-069', 169.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Nature Close-up #70', 'PHO-070', 149.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Aerial View #71', 'PHO-071', 399.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Underwater Photo #72', 'PHO-072', 449.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Night Sky #73', 'PHO-073', 299.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Food Photography #74', 'PHO-074', 129.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Fashion Shot #75', 'PHO-075', 249.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Documentary Photo #76', 'PHO-076', 279.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Long Exposure #77', 'PHO-077', 329.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Abstract Photography #78', 'PHO-078', 199.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Candid Moments #79', 'PHO-079', 179.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Vintage Photo #80', 'PHO-080', 259.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId4, @CategoryId1), ISNULL(@SubCatId4, @SubCatId1), @CurrentDate, @CurrentDate, 0);

PRINT 'Inserted products 61-80';

-- Products 81-100: Category 5
INSERT INTO Products (Title, SKU, Price, Status, ApprovalStatus, IsPublished, ShopId, CategoryId, SubCategoryId, CreatedAt, UpdatedAt, IsDeleted)
VALUES
('Handmade Jewelry #81', 'CRA-081', 79.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Knitted Blanket #82', 'CRA-082', 129.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Pottery Bowl #83', 'CRA-083', 89.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Macrame Wall Hanging #84', 'CRA-084', 69.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Leather Bag #85', 'CRA-085', 159.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Embroidery Art #86', 'CRA-086', 99.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Quilted Pillow #87', 'CRA-087', 49.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Beaded Necklace #88', 'CRA-088', 39.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Wooden Toy #89', 'CRA-089', 59.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Candle Set #90', 'CRA-090', 29.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Woven Basket #91', 'CRA-091', 79.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Stained Glass #92', 'CRA-092', 249.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Crocheted Scarf #93', 'CRA-093', 44.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Hand-painted Mug #94', 'CRA-094', 34.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Origami Art #95', 'CRA-095', 24.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Soap Making Kit #96', 'CRA-096', 39.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Pressed Flowers #97', 'CRA-097', 54.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Handmade Soap #98', 'CRA-098', 19.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Fabric Bookmark #99', 'CRA-099', 9.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0),
('Artisan Coaster Set #100', 'CRA-100', 29.99, 0, 1, 1, @ShopId, ISNULL(@CategoryId5, @CategoryId1), ISNULL(@SubCatId5, @SubCatId1), @CurrentDate, @CurrentDate, 0);

PRINT 'Inserted products 81-100';

-- Verify the insert
DECLARE @ProductCount INT;
SELECT @ProductCount = COUNT(*) FROM Products WHERE IsDeleted = 0;
PRINT 'Total products in database: ' + CAST(@ProductCount AS VARCHAR(10));

PRINT 'Successfully inserted 100 products!';
GO
