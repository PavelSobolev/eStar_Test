WITH DiscountedProducts AS (
  SELECT p.*,
         CASE WHEN pr.Price > @P0 THEN pr.Price * (1 - @P1/100.0) ELSE pr.Price END AS DiscountedPrice
  FROM Products p
  INNER JOIN Pricing pr ON p.SKU = pr.SKU
)
SELECT dp.StyleCode,
       MIN(dp.DiscountedPrice) AS LowestPrice,
       MAX(dp.DiscountedPrice) AS HighestPrice
FROM DiscountedProducts dp
INNER JOIN Products p ON dp.SKU = p.SKU
GROUP BY dp.StyleCode;

SELECT * from products 
join pricing on pricing.sku=products.sku
ORDER by StyleCode

/*
Declare @P0 FLOAT Set @P0 = 200
Declare @P1 FLOAT Set @P1 = 25

SELECT 
    StyleCode,
    MIN(CASE WHEN Price > @P0 THEN pr.Price * (1 - @P1/100.0) ELSE pr.Price END) AS [Lowest Price],
    MAX(CASE WHEN Price > @P0 THEN pr.Price * (1 - @P1/100.0) ELSE pr.Price END) AS [Highest Price]    
FROM 
    Products p
    INNER JOIN Pricing pr ON p.SKU = pr.SKU
GROUP BY StyleCode;
*/