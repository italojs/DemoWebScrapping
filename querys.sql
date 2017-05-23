create table Products(
id int identity(1,1),
urlPicture nvarchar(500),
[Description] nvarchar(500),
InitialBid nvarchar(500),													  
BiggestBid nvarchar(500),
QuantityBids int,
ProductName nvarchar(500)
)

//------------------------------------------------------------------------------
create procedure registerProducts 'url/url/url','description','1.000,00','2.000,00',22, 'productnme'
@UrlPicture  nvarchar(500),
@Description nvarchar(500),
@InitialBid  nvarchar(500),
@BiggestBid  nvarchar(500),
@QuantityBids int,
@ProductName nvarchar(500)
as

insert into [dbo].[Products](UrlPicture,[Description],InitialBid,BiggestBid,QuantityBids,ProductName) values(@UrlPicture,@Description,@InitialBid,@BiggestBid,@QuantityBids, @ProductName)

select * from products