
If OBJECT_ID('Products') IS NULL
Create Table Products(
	SKU NVARCHAR(10) PRIMARY KEY,
    [Name] NVARCHAR(32) NOT NULL,
    [Description] NVARCHAR(230),
    Brand NVARCHAR(35) NOT NULL,
    StyleCode NVARCHAR(7) NOT NULL,
    INDEX ProductBrandIndex NONCLUSTERED (Brand),
    INDEX ProductStyleCodeIndex NONCLUSTERED (StyleCode)
)

If OBJECT_ID('Pricing') IS NULL
Create Table Pricing(
	SKU NVARCHAR(10) PRIMARY KEY,
    Price money NOT NULL,
    FOREIGN KEY (SKU) REFERENCES Products(SKU)
)

If OBJECT_ID('Stock') IS NULL
Create Table Stock(	
	SKU NVARCHAR(10) NOT NULL FOREIGN KEY REFERENCES Products(SKU),
    [Location] NVARCHAR(25) NOT NULL,
    Quantity INT NOT NULL,    
	CONSTRAINT PK_Stock PRIMARY KEY CLUSTERED (SKU, [Location]),
    INDEX StockLocationIndex NONCLUSTERED ([Location])
)