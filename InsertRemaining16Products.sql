-- SQL Script to Insert Additional 16 Products (to reach 100 total)
-- Run this script on the ArtEva database

USE ArtEva;
GO

DECLARE @ShopId INT;
DECLARE @CategoryId1 INT, @CategoryId2 INT, @CategoryId3 INT;
DECLARE @SubCatId1 INT, @SubCatId2 INT, @SubCatId3 INT;
DECLARE @CurrentDate DATETIME = GETDATE();

-- Get first active shop
SELECT TOP 1 @ShopId = Id FROM Shops WHERE IsDeleted = 0;

-- Get category IDs
SELECT TOP 1 @CategoryId1 = Id FROM Categories WHERE IsDeleted = 0 ORDER BY Id;
SELECT @CategoryId2 = Id FROM Categories WHERE IsDeleted = 0 AND Id > @CategoryId1 ORDER BY Id OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;
SELECT @CategoryId3 = Id FROM Categories WHERE IsDeleted = 0 AND Id > @CategoryId2 ORDER BY Id OFFSET 0 ROWS FETCH NEXT 1 ROWS ONLY;

-- Get subcategory IDs
SELECT TOP 1 @SubCatId1 = Id FROM SubCategories WHERE IsDeleted = 0 AND CategoryId = @CategoryId1;
SELECT TOP 1 @SubCatId2 = Id FROM SubCategories WHERE IsDeleted = 0 AND CategoryId = @CategoryId2;
SELECT TOP 1 @SubCatId3 = Id FROM SubCategories WHERE IsDeleted = 0 AND CategoryId = @CategoryId3;

PRINT 'Inserting additional 16 products to reach 100 total...';

-- Insert remaining 16 products with unique SKUs
INSERT INTO Products (Title, SKU, Price, Status, ApprovalStatus, IsPublished, ShopId, CategoryId, SubCategoryId, CreatedAt, UpdatedAt, IsDeleted)
VALUES
('Expressionist Painting #101', 'ART-101', 379.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Vintage Poster Print #102', 'ART-102', 89.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Tribal Art Piece #103', 'ART-103', 269.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Renaissance Style #104', 'ART-104', 499.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Modern Abstract #105', 'ART-105', 219.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Cubist Composition #106', 'ART-106', 329.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Art Deco Design #107', 'ART-107', 289.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Folk Art Collection #108', 'ART-108', 159.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Contemporary Mixed Media #109', 'ART-109', 399.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Graffiti Art Print #110', 'ART-110', 179.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Japanese Ink Painting #111', 'ART-111', 449.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('African Art Inspired #112', 'ART-112', 249.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Framed Canvas Art #113', 'ART-113', 299.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Limited Edition Print #114', 'ART-114', 599.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Collage Artwork #115', 'ART-115', 189.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0),
('Panoramic Landscape #116', 'ART-116', 549.99, 0, 1, 1, @ShopId, @CategoryId1, @SubCatId1, @CurrentDate, @CurrentDate, 0);

-- Verify the total count
DECLARE @ProductCount INT;
SELECT @ProductCount = COUNT(*) FROM Products WHERE IsDeleted = 0;
PRINT 'Total products in database: ' + CAST(@ProductCount AS VARCHAR(10));

IF @ProductCount >= 100
BEGIN
    PRINT 'SUCCESS: Database now contains 100+ products!';
END
ELSE
BEGIN
    PRINT 'Current product count: ' + CAST(@ProductCount AS VARCHAR(10));
END

GO
