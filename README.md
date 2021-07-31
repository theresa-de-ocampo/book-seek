# POS System for Books

## Overview
A fictional company named **BookSeek** was used for this program, which is composed of three portals: (1) POS, (2) Inventory, and (3) Audits. The first portal is where purchases actually occurs. The inventory portal handles the stock management while the last portal consists of sales history, and ledger.

## Features
1. POS
	1. Punch a book into a transaction.
	2. Void an item either through the list of all books, or list of punched items.
	3. Checks out of stock books.
	4. Checks insufficient quantity.
	5. Checks if cash is insufficient.
	6. Void an entire transaction.
	7. On payment, checks if list of punched items is empty.
	8. Prints receipts upon successful purchase.
2. Inventory
	1. Add a book.
	2. Edit a book.
	3. Delete a book.
	4. Restore a deleted book.
	5. Change stock quantity of a book.
	6. Checks ISBN upon add and edit.
	7. Lists active books.
	8. Lists deactivated books.
	9. Adds a record to the ledger upon adding a book, editing a book's quantity, or deactivating a product.
3. Audits
	1. Today's sales, and earnings figures along with the all time best seller.
	2. Lists all sales.
	3. Provision to re-print receipts.
	4. Provision to return an item following the 7-day policy.
		1. Cash Back
		2. Exchange Items
			1. Out of stock books are not shown as an option.
			2. Checks insufficient quantity.
			3. If the new item(s) to be exchanged costs less than the original purchase, the difference will be refunded.
			4. If the new items costs more, customer is charged for the difference in price.
			5. Checks if cash is insufficient.
		3. General
			1. If only 1 item was bought, it is automatically loaded to be returned.
			2. Number of items returned could never be greater than what was bought.
			3. Automatically loads item if quantity was changed.
			4. Will not proceed with returning books if no item was checked.
			5. Updates lists of sales according to: (1) partial or full refund, or (2) cash back or exchange of items.
			6. Provision for inventory restock.
			7. If invetory was not restocked after return, a record is added to the ledger.
4. General
	1. Search for a product, transaction, ledger entry, etc.
	2. Validations for numeric inputs i.e., accounts non-numeric inputs or negative numbers.
	3. Checks invalid formats of string inputs.
	4. Checks for incomplete inputs on forms.
	5. Validation for date and time fields.

## Requirements
- Visual Studio 2019
	- .NET Framework v4.7.2. or higher
	- Entity Framework v6.0.0.0 or higher
- Microsoft SQL Server 2019

## Installation
1. Clone the repository.
	```bash
		git clone https://github.com/theresa-de-ocampo/book-seek.git
	```
2. Execute the commands in ```setup.sql``` through MS SQL Server.
3. In ```BookSeek/app.config```, replace all occurrences of ```lg-pc``` with your server name; which is by default your computer name.
4. Build and run the program.

## Sample Outputs
![Application Screenshots](sample-outputs.gif)