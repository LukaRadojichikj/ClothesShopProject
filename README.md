# ClothesShopProject

Made in ASP.NET Core 7.0 MVC.

How to Run  
Ensure you have Entity Framework installed.  
In the Package Manager Console, run:

Update-Database

There are already 2 users:

Admin role with credentials:

-Username: "admin@admin.com"

-Password: "Admin123"

User role with credentials:

-Username: "user@user.com"

-Password: "User1234"

This is a refined and improved version of my earlier project. Key updates and enhancements include:

- Dynamic Cart Badge: Live cart item count with AJAX support.
- Database Fixes: Resolved issues related to foreign key constraints and relationships.
- Image Handling: Uploads now support automatic file renaming to avoid duplicates.
- Delete Improvements: Deleting items now properly removes associated images.
- Cascade Deletes: Configured to clean up related entities automatically.
- Controller Cleanup: Removed the unused CartController and associated views to streamline codebase.
