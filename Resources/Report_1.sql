SELECT Products.SKU, Products.Name, Products.Description, Products.Brand, Pricing.Price as Price, Stock.[Location] as [Location] From Products
    JOIN Pricing ON Products.SKU = Pricing.SKU
    JOIN Stock ON Products.SKU = Stock.SKU
Where Stock.[Location] in ('Subassembly', 'Finished Goods Storage') AND 
    Products.Brand in ('ACME Corporation', 'Inner City Bikes')