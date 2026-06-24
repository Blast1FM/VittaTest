USE OrderAccountingDB;
GO

SET NOCOUNT ON;

BEGIN
	-- Очистка таблиц
	DELETE FROM dbo.Payments;
	DELETE FROM dbo.Orders;
	DELETE FROM dbo.CashInflows;
	
	-- ПАРАМЕТРЫ ГЕНЕРАЦИИ

	DECLARE @NumberOfOrders     INT = 3000;
	DECLARE @NumberOfInflows    INT = 1500;
	DECLARE @MaxDaysBack        INT = 90;
	
	DECLARE @i INT;
	
	PRINT '=== Начало генерации тестовых данных ===';
	PRINT CONCAT('Заказов: ', @NumberOfOrders, ' | Поступлений: ', @NumberOfInflows);
	PRINT '------------------------------------------------';
	
	-- 1. Генерация заказов
	PRINT 'Генерация заказов...';
	
	SET @i = 1;
	
	WHILE @i <= @NumberOfOrders
	BEGIN
	    DECLARE @OrderDate     DATETIME = DATEADD(DAY, - (ABS(CHECKSUM(NEWID())) % @MaxDaysBack), GETDATE());
	    DECLARE @Amount        DECIMAL(18,2) = ROUND(500.00 + (RAND() * 49500.00), 2);
	    DECLARE @PaidPercent   DECIMAL(5,4) = RAND() * 0.95;
	    DECLARE @PaidAmount    DECIMAL(18,2) = ROUND(@Amount * @PaidPercent, 2);
	
	    INSERT INTO dbo.Orders (OrderDate, Amount, PaidAmount)
	    VALUES (@OrderDate, @Amount, @PaidAmount);
	
	    IF @i % 1000 = 0
	        PRINT CONCAT('   → Сгенерировано заказов: ', @i);
	
	    SET @i = @i + 1;
	END
	
	PRINT CONCAT('Генерация заказов завершена. Всего: ', @NumberOfOrders);
	PRINT '------------------------------------------------';
	

	-- Генерация приходов (CashInflows)
	PRINT 'Генерация приходов...';
	
	SET @i = 1;
	
	WHILE @i <= @NumberOfInflows
	BEGIN
	    DECLARE @InflowDate    DATETIME = DATEADD(DAY, - (ABS(CHECKSUM(NEWID())) % @MaxDaysBack), GETDATE());
	    DECLARE @InflowAmount  DECIMAL(18,2) = ROUND(1000.00 + (RAND() * 999000.00), 2);
	    DECLARE @UsedPercent   DECIMAL(5,4) = RAND() * 0.70;
	    DECLARE @Remaining     DECIMAL(18,2) = ROUND(@InflowAmount * (1 - @UsedPercent), 2);
	
	    INSERT INTO dbo.CashInflows (InflowDate, Amount, Remaining)
	    VALUES (@InflowDate, @InflowAmount, @Remaining);
	
	    IF @i % 500 = 0
	        PRINT CONCAT('   → Сгенерировано приходов: ', @i);
	
	    SET @i = @i + 1;
	END
	
	PRINT CONCAT('Генерация поступлений завершена. Всего: ', @NumberOfInflows);
	PRINT '------------------------------------------------';

	-- Итоговая статистика
	PRINT '=== Генерация успешно завершена ===';
	
	SELECT 
	    COUNT(*) AS TotalOrders,
	    MIN(OrderDate) AS OldestOrder,
	    MAX(OrderDate) AS NewestOrder,
	    AVG(Amount) AS AvgOrderAmount,
	    SUM(Amount) AS TotalAmount,
	    SUM(PaidAmount) AS TotalPaid
	FROM dbo.Orders;
	
	SELECT 
	    COUNT(*) AS TotalInflows,
	    MIN(InflowDate) AS OldestInflow,
	    MAX(InflowDate) AS NewestInflow,
	    AVG(Amount) AS AvgInflowAmount,
	    SUM(Remaining) AS TotalRemaining
	FROM dbo.CashInflows;
END;
GO