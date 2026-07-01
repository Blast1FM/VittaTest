-- Создание БД
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'OrderAccountingDB')
BEGIN
    CREATE DATABASE OrderAccountingDB;
END;
GO

USE OrderAccountingDB;
GO

-- Заказы
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Orders (
        OrderNumber INT IDENTITY(1,1) PRIMARY KEY,
        OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
        Amount DECIMAL(18,2) NOT NULL CHECK (Amount >= 0),
        PaidAmount DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (PaidAmount >= 0),
        RowVersion ROWVERSION NOT NULL
    );
END;
GO

-- Приход денег
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CashInflows' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.CashInflows (
        InflowNumber INT IDENTITY(1,1) PRIMARY KEY,
        InflowDate DATETIME NOT NULL DEFAULT GETDATE(),
        Amount DECIMAL(18,2) NOT NULL CHECK (Amount > 0),
        Remaining DECIMAL(18,2) NOT NULL DEFAULT 0 CHECK (Remaining >= 0),
        RowVersion ROWVERSION NOT NULL
    );
END;
GO

-- Платежи
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Payments' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Payments (
        PaymentID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        OrderNumber INT NOT NULL,
        InflowNumber INT NOT NULL,
        PaymentAmount DECIMAL(18,2) NOT NULL CHECK (PaymentAmount > 0),
        PaymentDate DATETIME NOT NULL DEFAULT GETDATE(),
        ExpectedOrderVersion VARBINARY(8) NOT NULL,
        ExpectedInflowVersion VARBINARY(8) NOT NULL,
        CONSTRAINT FK_Payments_Orders FOREIGN KEY (OrderNumber) REFERENCES dbo.Orders(OrderNumber),
        CONSTRAINT FK_Payments_CashInflows FOREIGN KEY (InflowNumber) REFERENCES dbo.CashInflows(InflowNumber)
    );
END;
GO

-- Индекс по FK Payments_OrderNumber
IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'IX_Payments_OrderNumber' 
      AND object_id = OBJECT_ID('dbo.Payments')
)
BEGIN
    CREATE INDEX IX_Payments_OrderNumber 
    ON dbo.Payments(OrderNumber);
END;
GO

-- Индекс по FK Payments_InflowNumber
IF NOT EXISTS (
    SELECT 1 
    FROM sys.indexes 
    WHERE name = 'IX_Payments_InflowNumber' 
      AND object_id = OBJECT_ID('dbo.Payments')
)
BEGIN
    CREATE INDEX IX_Payments_InflowNumber 
    ON dbo.Payments(InflowNumber);
END;
GO

-- Триггер на новые платежи
CREATE OR ALTER TRIGGER TR_Payments_ApplyPayment
ON dbo.Payments
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @InsertedCount INT = (SELECT COUNT(*) FROM inserted);

    -- Обновление заказов
    UPDATE o
    SET PaidAmount = PaidAmount + i.PaymentAmount
    FROM dbo.Orders o
    INNER JOIN inserted i ON o.OrderNumber = i.OrderNumber
    WHERE o.RowVersion = i.ExpectedOrderVersion
      AND o.PaidAmount + i.PaymentAmount <= o.Amount;

    IF @@ROWCOUNT <> @InsertedCount
        THROW 51001, 'Оплата не проведена: заказ изменён или превышена сумма.', 1;

    -- Обновление приходов
    UPDATE ci
    SET Remaining = Remaining - i.PaymentAmount
    FROM dbo.CashInflows ci
    INNER JOIN inserted i ON ci.InflowNumber = i.InflowNumber
    WHERE ci.RowVersion = i.ExpectedInflowVersion
      AND ci.Remaining >= i.PaymentAmount;

    IF @@ROWCOUNT <> @InsertedCount
        THROW 51002, 'Оплата не проведена: приход изменён или недостаточно средств.', 1;
END;
GO

-- Триггер на проверку остатка нового прихода
-- Если остаток не указан, он становится равным сумме прихода
CREATE OR ALTER TRIGGER TR_CashInflows_ValidateRemaining
ON dbo.CashInflows
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    UPDATE ci
    SET ci.Remaining = ci.Amount
    FROM dbo.CashInflows ci
    INNER JOIN inserted i ON ci.InflowNumber = i.InflowNumber
    WHERE i.Remaining = 0;
END;
GO