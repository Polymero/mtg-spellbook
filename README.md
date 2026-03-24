
# Spellbox




### To-do
-----------

> Release test build

> Add update logic to deck contents page

> Update binder/deck cover image selection

> Add user profile switching with separate dbs

> Add import/export logic for added ProductIds

> Add format-specific deck legality logic service

> Add paged viewer to CardGridView

> Reset db migrations


### Changelog
---

> ##### 2026-03-17
> Additions:
> - Added card legalities to OracleDb (+ ~2MB)
> - Added final import logic for importing .csv files into collection.
> - Added logic to store user ProductIds in the respective MarketDb to survive OracleDb drop.
> Fixes:
> - Matched deck UI to binder UI and added board tabs.
> - Added small batch of missing CardMarket ProductIds.
> - Fixed MudSelect display bug after changing value outside element itself by adding ToString override of the displayed class.
