# EfCoreDisconnectedPerformance
Benchmarking different disconnected entity scenarios
This came up after a talk about EF Core and optimistic concurrency in disconnected scenarios.

# Scenarios
1. Force update
   Just take whatever the client gave us and update.
   If the update fails, we will return a 409 - Conflict.
2. Re-Query before updating
   Query the database and only update what has changed.
   If the entity does not exist on the re-query, we will return 404 - Not Found.
   If the entity has been changed between the initial query and the re-query, we will return 409 - Conflict.
   If the entity was changed or deleted between the re-query on the update, we will return 409 - Conflict.
   Note that we cannot distinguish whether the entity has been modified or deleted between the re-query and the update.
3. Re-Query and lock
   Query the database, lock the row in the database and only update what has changed.
   If the entity does not exist on the re-query, we will return 404 - Not Found.
   If the entity has been changed between the initial query and the re-query, we will return 409 - Conflict.
   If the entity was changed or deleted between the re-query on the update, we will return 409 - Conflict.
   The entity cannot be deleted between the re-query and the update, because we are holding a lock on it.
4. Re-Query 2x
   Query the database and only update what has changed.
   If the entity does not exist on the re-query, we will return 404 - Not Found.
   If the entity has been changed between the initial query and the re-query, we will return 409 - Conflict.
   If the entity was changed or deleted between the re-query on the update, we will query the database again and either return 404 or 409 based on the result.
   This implies that the keys of the entities cannot be used again once they have been deleted.

# Results
## Using LocalDB

| Method                             | Entries | ConcurrentUsers | PercentageOfDeletes | Mean       | Error | Ratio |
|----------------------------------- |--------: |----------------: |--------------------: |-----------:|------:|------:|
| UpdateForceAsync                   | 100     | 2               | 0                   |   475.7 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 100     | 2               | 0                   |   450.8 ms |    NA |  0.95 |
| UpdateReQueryAsync                 | 100     | 2               | 0                   |   479.8 ms |    NA |  1.01 |
| UpdateReQueryReturnsExistingAsync  | 100     | 2               | 0                   |   531.3 ms |    NA |  1.12 |
| UpdateReQuery2xAsync               | 100     | 2               | 0                   |   498.5 ms |    NA |  1.05 |
| UpdateReQuery2xReturnExistingAsync | 100     | 2               | 0                   |   499.3 ms |    NA |  1.05 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 100     | 2               | 0.1                 |   466.5 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 100     | 2               | 0.1                 |   497.3 ms |    NA |  1.07 |
| UpdateReQueryAsync                 | 100     | 2               | 0.1                 |   474.4 ms |    NA |  1.02 |
| UpdateReQueryReturnsExistingAsync  | 100     | 2               | 0.1                 |   512.0 ms |    NA |  1.10 |
| UpdateReQuery2xAsync               | 100     | 2               | 0.1                 |   506.0 ms |    NA |  1.08 |
| UpdateReQuery2xReturnExistingAsync | 100     | 2               | 0.1                 |   487.3 ms |    NA |  1.04 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 100     | 2               | 0.5                 |   475.9 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 100     | 2               | 0.5                 |   437.2 ms |    NA |  0.92 |
| UpdateReQueryAsync                 | 100     | 2               | 0.5                 |   467.1 ms |    NA |  0.98 |
| UpdateReQueryReturnsExistingAsync  | 100     | 2               | 0.5                 |   471.0 ms |    NA |  0.99 |
| UpdateReQuery2xAsync               | 100     | 2               | 0.5                 |   460.2 ms |    NA |  0.97 |
| UpdateReQuery2xReturnExistingAsync | 100     | 2               | 0.5                 |   486.7 ms |    NA |  1.02 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 100     | 2               | 0.9                 |   457.3 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 100     | 2               | 0.9                 |   425.9 ms |    NA |  0.93 |
| UpdateReQueryAsync                 | 100     | 2               | 0.9                 |   453.8 ms |    NA |  0.99 |
| UpdateReQueryReturnsExistingAsync  | 100     | 2               | 0.9                 |   468.9 ms |    NA |  1.03 |
| UpdateReQuery2xAsync               | 100     | 2               | 0.9                 |   446.3 ms |    NA |  0.98 |
| UpdateReQuery2xReturnExistingAsync | 100     | 2               | 0.9                 |   451.0 ms |    NA |  0.99 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 100     | 10              | 0                   |   708.6 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 100     | 10              | 0                   |   673.7 ms |    NA |  0.95 |
| UpdateReQueryAsync                 | 100     | 10              | 0                   |   784.6 ms |    NA |  1.11 |
| UpdateReQueryReturnsExistingAsync  | 100     | 10              | 0                   |   698.6 ms |    NA |  0.99 |
| UpdateReQuery2xAsync               | 100     | 10              | 0                   |   826.9 ms |    NA |  1.17 |
| UpdateReQuery2xReturnExistingAsync | 100     | 10              | 0                   |   759.9 ms |    NA |  1.07 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 100     | 10              | 0.1                 |   802.7 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 100     | 10              | 0.1                 |   649.7 ms |    NA |  0.81 |
| UpdateReQueryAsync                 | 100     | 10              | 0.1                 |   766.7 ms |    NA |  0.96 |
| UpdateReQueryReturnsExistingAsync  | 100     | 10              | 0.1                 |   742.8 ms |    NA |  0.93 |
| UpdateReQuery2xAsync               | 100     | 10              | 0.1                 |   751.0 ms |    NA |  0.94 |
| UpdateReQuery2xReturnExistingAsync | 100     | 10              | 0.1                 |   797.0 ms |    NA |  0.99 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 100     | 10              | 0.5                 |   664.7 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 100     | 10              | 0.5                 |   643.7 ms |    NA |  0.97 |
| UpdateReQueryAsync                 | 100     | 10              | 0.5                 |   666.7 ms |    NA |  1.00 |
| UpdateReQueryReturnsExistingAsync  | 100     | 10              | 0.5                 |   752.4 ms |    NA |  1.13 |
| UpdateReQuery2xAsync               | 100     | 10              | 0.5                 |   660.8 ms |    NA |  0.99 |
| UpdateReQuery2xReturnExistingAsync | 100     | 10              | 0.5                 |   790.4 ms |    NA |  1.19 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 100     | 10              | 0.9                 |   721.9 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 100     | 10              | 0.9                 |   742.0 ms |    NA |  1.03 |
| UpdateReQueryAsync                 | 100     | 10              | 0.9                 |   731.8 ms |    NA |  1.01 |
| UpdateReQueryReturnsExistingAsync  | 100     | 10              | 0.9                 |   727.3 ms |    NA |  1.01 |
| UpdateReQuery2xAsync               | 100     | 10              | 0.9                 |   736.6 ms |    NA |  1.02 |
| UpdateReQuery2xReturnExistingAsync | 100     | 10              | 0.9                 |   731.2 ms |    NA |  1.01 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 100     | 100             | 0                   | 6,201.9 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 100     | 100             | 0                   | 6,155.4 ms |    NA |  0.99 |
| UpdateReQueryAsync                 | 100     | 100             | 0                   | 6,024.3 ms |    NA |  0.97 |
| UpdateReQueryReturnsExistingAsync  | 100     | 100             | 0                   | 5,829.7 ms |    NA |  0.94 |
| UpdateReQuery2xAsync               | 100     | 100             | 0                   | 5,830.2 ms |    NA |  0.94 |
| UpdateReQuery2xReturnExistingAsync | 100     | 100             | 0                   | 5,881.0 ms |    NA |  0.95 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 100     | 100             | 0.1                 | 5,678.5 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 100     | 100             | 0.1                 | 5,020.3 ms |    NA |  0.88 |
| UpdateReQueryAsync                 | 100     | 100             | 0.1                 | 4,838.6 ms |    NA |  0.85 |
| UpdateReQueryReturnsExistingAsync  | 100     | 100             | 0.1                 | 4,422.4 ms |    NA |  0.78 |
| UpdateReQuery2xAsync               | 100     | 100             | 0.1                 | 4,729.1 ms |    NA |  0.83 |
| UpdateReQuery2xReturnExistingAsync | 100     | 100             | 0.1                 | 4,638.4 ms |    NA |  0.82 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 100     | 100             | 0.5                 | 5,182.5 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 100     | 100             | 0.5                 | 4,737.6 ms |    NA |  0.91 |
| UpdateReQueryAsync                 | 100     | 100             | 0.5                 | 4,904.4 ms |    NA |  0.95 |
| UpdateReQueryReturnsExistingAsync  | 100     | 100             | 0.5                 | 4,735.8 ms |    NA |  0.91 |
| UpdateReQuery2xAsync               | 100     | 100             | 0.5                 | 4,845.1 ms |    NA |  0.93 |
| UpdateReQuery2xReturnExistingAsync | 100     | 100             | 0.5                 | 4,714.2 ms |    NA |  0.91 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 100     | 100             | 0.9                 | 5,025.3 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 100     | 100             | 0.9                 | 4,776.2 ms |    NA |  0.95 |
| UpdateReQueryAsync                 | 100     | 100             | 0.9                 | 4,768.9 ms |    NA |  0.95 |
| UpdateReQueryReturnsExistingAsync  | 100     | 100             | 0.9                 | 5,109.3 ms |    NA |  1.02 |
| UpdateReQuery2xAsync               | 100     | 100             | 0.9                 | 5,080.6 ms |    NA |  1.01 |
| UpdateReQuery2xReturnExistingAsync | 100     | 100             | 0.9                 | 4,970.4 ms |    NA |  0.99 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 10000   | 2               | 0                   |   419.6 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 10000   | 2               | 0                   |   430.1 ms |    NA |  1.03 |
| UpdateReQueryAsync                 | 10000   | 2               | 0                   |   469.8 ms |    NA |  1.12 |
| UpdateReQueryReturnsExistingAsync  | 10000   | 2               | 0                   |   503.7 ms |    NA |  1.20 |
| UpdateReQuery2xAsync               | 10000   | 2               | 0                   |   503.5 ms |    NA |  1.20 |
| UpdateReQuery2xReturnExistingAsync | 10000   | 2               | 0                   |   492.1 ms |    NA |  1.17 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 10000   | 2               | 0.1                 |   430.8 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 10000   | 2               | 0.1                 |   430.0 ms |    NA |  1.00 |
| UpdateReQueryAsync                 | 10000   | 2               | 0.1                 |   479.6 ms |    NA |  1.11 |
| UpdateReQueryReturnsExistingAsync  | 10000   | 2               | 0.1                 |   572.2 ms |    NA |  1.33 |
| UpdateReQuery2xAsync               | 10000   | 2               | 0.1                 |   521.8 ms |    NA |  1.21 |
| UpdateReQuery2xReturnExistingAsync | 10000   | 2               | 0.1                 |   489.3 ms |    NA |  1.14 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 10000   | 2               | 0.5                 |   484.7 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 10000   | 2               | 0.5                 |   469.2 ms |    NA |  0.97 |
| UpdateReQueryAsync                 | 10000   | 2               | 0.5                 |   511.5 ms |    NA |  1.06 |
| UpdateReQueryReturnsExistingAsync  | 10000   | 2               | 0.5                 |   508.2 ms |    NA |  1.05 |
| UpdateReQuery2xAsync               | 10000   | 2               | 0.5                 |   489.5 ms |    NA |  1.01 |
| UpdateReQuery2xReturnExistingAsync | 10000   | 2               | 0.5                 |   521.9 ms |    NA |  1.08 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 10000   | 2               | 0.9                 |   458.8 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 10000   | 2               | 0.9                 |   459.8 ms |    NA |  1.00 |
| UpdateReQueryAsync                 | 10000   | 2               | 0.9                 |   444.9 ms |    NA |  0.97 |
| UpdateReQueryReturnsExistingAsync  | 10000   | 2               | 0.9                 |   451.4 ms |    NA |  0.98 |
| UpdateReQuery2xAsync               | 10000   | 2               | 0.9                 |   455.2 ms |    NA |  0.99 |
| UpdateReQuery2xReturnExistingAsync | 10000   | 2               | 0.9                 |   448.9 ms |    NA |  0.98 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 10000   | 10              | 0                   |   685.0 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 10000   | 10              | 0                   |   693.1 ms |    NA |  1.01 |
| UpdateReQueryAsync                 | 10000   | 10              | 0                   |   741.7 ms |    NA |  1.08 |
| UpdateReQueryReturnsExistingAsync  | 10000   | 10              | 0                   |   734.1 ms |    NA |  1.07 |
| UpdateReQuery2xAsync               | 10000   | 10              | 0                   |   715.1 ms |    NA |  1.04 |
| UpdateReQuery2xReturnExistingAsync | 10000   | 10              | 0                   |   663.2 ms |    NA |  0.97 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 10000   | 10              | 0.1                 |   675.1 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 10000   | 10              | 0.1                 |   673.9 ms |    NA |  1.00 |
| UpdateReQueryAsync                 | 10000   | 10              | 0.1                 |   695.5 ms |    NA |  1.03 |
| UpdateReQueryReturnsExistingAsync  | 10000   | 10              | 0.1                 |   712.6 ms |    NA |  1.06 |
| UpdateReQuery2xAsync               | 10000   | 10              | 0.1                 |   694.7 ms |    NA |  1.03 |
| UpdateReQuery2xReturnExistingAsync | 10000   | 10              | 0.1                 |   726.8 ms |    NA |  1.08 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 10000   | 10              | 0.5                 |   657.8 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 10000   | 10              | 0.5                 |   656.4 ms |    NA |  1.00 |
| UpdateReQueryAsync                 | 10000   | 10              | 0.5                 |   680.1 ms |    NA |  1.03 |
| UpdateReQueryReturnsExistingAsync  | 10000   | 10              | 0.5                 |   674.2 ms |    NA |  1.03 |
| UpdateReQuery2xAsync               | 10000   | 10              | 0.5                 |   685.6 ms |    NA |  1.04 |
| UpdateReQuery2xReturnExistingAsync | 10000   | 10              | 0.5                 |   663.5 ms |    NA |  1.01 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 10000   | 10              | 0.9                 |   706.6 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 10000   | 10              | 0.9                 |   671.0 ms |    NA |  0.95 |
| UpdateReQueryAsync                 | 10000   | 10              | 0.9                 |   659.9 ms |    NA |  0.93 |
| UpdateReQueryReturnsExistingAsync  | 10000   | 10              | 0.9                 |   683.6 ms |    NA |  0.97 |
| UpdateReQuery2xAsync               | 10000   | 10              | 0.9                 |   682.4 ms |    NA |  0.97 |
| UpdateReQuery2xReturnExistingAsync | 10000   | 10              | 0.9                 |   638.1 ms |    NA |  0.90 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 10000   | 100             | 0                   | 5,950.0 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 10000   | 100             | 0                   | 5,996.1 ms |    NA |  1.01 |
| UpdateReQueryAsync                 | 10000   | 100             | 0                   | 6,098.3 ms |    NA |  1.02 |
| UpdateReQueryReturnsExistingAsync  | 10000   | 100             | 0                   | 5,862.1 ms |    NA |  0.99 |
| UpdateReQuery2xAsync               | 10000   | 100             | 0                   | 5,913.0 ms |    NA |  0.99 |
| UpdateReQuery2xReturnExistingAsync | 10000   | 100             | 0                   | 5,739.6 ms |    NA |  0.96 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 10000   | 100             | 0.1                 | 5,510.3 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 10000   | 100             | 0.1                 | 4,818.3 ms |    NA |  0.87 |
| UpdateReQueryAsync                 | 10000   | 100             | 0.1                 | 4,745.9 ms |    NA |  0.86 |
| UpdateReQueryReturnsExistingAsync  | 10000   | 100             | 0.1                 | 4,654.8 ms |    NA |  0.84 |
| UpdateReQuery2xAsync               | 10000   | 100             | 0.1                 | 4,581.8 ms |    NA |  0.83 |
| UpdateReQuery2xReturnExistingAsync | 10000   | 100             | 0.1                 | 4,435.0 ms |    NA |  0.80 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 10000   | 100             | 0.5                 | 5,078.6 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 10000   | 100             | 0.5                 | 4,745.5 ms |    NA |  0.93 |
| UpdateReQueryAsync                 | 10000   | 100             | 0.5                 | 4,564.1 ms |    NA |  0.90 |
| UpdateReQueryReturnsExistingAsync  | 10000   | 100             | 0.5                 | 4,455.7 ms |    NA |  0.88 |
| UpdateReQuery2xAsync               | 10000   | 100             | 0.5                 | 4,570.9 ms |    NA |  0.90 |
| UpdateReQuery2xReturnExistingAsync | 10000   | 100             | 0.5                 | 4,382.4 ms |    NA |  0.86 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 10000   | 100             | 0.9                 | 4,615.2 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 10000   | 100             | 0.9                 | 4,756.0 ms |    NA |  1.03 |
| UpdateReQueryAsync                 | 10000   | 100             | 0.9                 | 4,815.1 ms |    NA |  1.04 |
| UpdateReQueryReturnsExistingAsync  | 10000   | 100             | 0.9                 | 4,755.7 ms |    NA |  1.03 |
| UpdateReQuery2xAsync               | 10000   | 100             | 0.9                 | 4,894.2 ms |    NA |  1.06 |
| UpdateReQuery2xReturnExistingAsync | 10000   | 100             | 0.9                 | 4,894.2 ms |    NA |  1.06 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 1000000 | 2               | 0                   |   417.8 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 1000000 | 2               | 0                   |   404.7 ms |    NA |  0.97 |
| UpdateReQueryAsync                 | 1000000 | 2               | 0                   |   496.5 ms |    NA |  1.19 |
| UpdateReQueryReturnsExistingAsync  | 1000000 | 2               | 0                   |   472.8 ms |    NA |  1.13 |
| UpdateReQuery2xAsync               | 1000000 | 2               | 0                   |   492.8 ms |    NA |  1.18 |
| UpdateReQuery2xReturnExistingAsync | 1000000 | 2               | 0                   |   597.2 ms |    NA |  1.43 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 1000000 | 2               | 0.1                 |   437.8 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 1000000 | 2               | 0.1                 |   400.7 ms |    NA |  0.92 |
| UpdateReQueryAsync                 | 1000000 | 2               | 0.1                 |   432.2 ms |    NA |  0.99 |
| UpdateReQueryReturnsExistingAsync  | 1000000 | 2               | 0.1                 |   439.2 ms |    NA |  1.00 |
| UpdateReQuery2xAsync               | 1000000 | 2               | 0.1                 |   436.8 ms |    NA |  1.00 |
| UpdateReQuery2xReturnExistingAsync | 1000000 | 2               | 0.1                 |   476.6 ms |    NA |  1.09 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 1000000 | 2               | 0.5                 |   386.0 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 1000000 | 2               | 0.5                 |   386.7 ms |    NA |  1.00 |
| UpdateReQueryAsync                 | 1000000 | 2               | 0.5                 |   448.7 ms |    NA |  1.16 |
| UpdateReQueryReturnsExistingAsync  | 1000000 | 2               | 0.5                 |   421.5 ms |    NA |  1.09 |
| UpdateReQuery2xAsync               | 1000000 | 2               | 0.5                 |   448.3 ms |    NA |  1.16 |
| UpdateReQuery2xReturnExistingAsync | 1000000 | 2               | 0.5                 |   486.6 ms |    NA |  1.26 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 1000000 | 2               | 0.9                 |   403.4 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 1000000 | 2               | 0.9                 |   443.8 ms |    NA |  1.10 |
| UpdateReQueryAsync                 | 1000000 | 2               | 0.9                 |   396.5 ms |    NA |  0.98 |
| UpdateReQueryReturnsExistingAsync  | 1000000 | 2               | 0.9                 |   398.2 ms |    NA |  0.99 |
| UpdateReQuery2xAsync               | 1000000 | 2               | 0.9                 |   397.9 ms |    NA |  0.99 |
| UpdateReQuery2xReturnExistingAsync | 1000000 | 2               | 0.9                 |   441.8 ms |    NA |  1.10 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 1000000 | 10              | 0                   |   572.6 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 1000000 | 10              | 0                   |   637.6 ms |    NA |  1.11 |
| UpdateReQueryAsync                 | 1000000 | 10              | 0                   |   644.9 ms |    NA |  1.13 |
| UpdateReQueryReturnsExistingAsync  | 1000000 | 10              | 0                   |   614.2 ms |    NA |  1.07 |
| UpdateReQuery2xAsync               | 1000000 | 10              | 0                   |   636.5 ms |    NA |  1.11 |
| UpdateReQuery2xReturnExistingAsync | 1000000 | 10              | 0                   |   604.1 ms |    NA |  1.05 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 1000000 | 10              | 0.1                 |   583.1 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 1000000 | 10              | 0.1                 |   566.0 ms |    NA |  0.97 |
| UpdateReQueryAsync                 | 1000000 | 10              | 0.1                 |   612.8 ms |    NA |  1.05 |
| UpdateReQueryReturnsExistingAsync  | 1000000 | 10              | 0.1                 |   610.5 ms |    NA |  1.05 |
| UpdateReQuery2xAsync               | 1000000 | 10              | 0.1                 |   658.8 ms |    NA |  1.13 |
| UpdateReQuery2xReturnExistingAsync | 1000000 | 10              | 0.1                 |   613.3 ms |    NA |  1.05 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 1000000 | 10              | 0.5                 |   580.0 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 1000000 | 10              | 0.5                 |   576.1 ms |    NA |  0.99 |
| UpdateReQueryAsync                 | 1000000 | 10              | 0.5                 |   602.9 ms |    NA |  1.04 |
| UpdateReQueryReturnsExistingAsync  | 1000000 | 10              | 0.5                 |   559.8 ms |    NA |  0.97 |
| UpdateReQuery2xAsync               | 1000000 | 10              | 0.5                 |   586.3 ms |    NA |  1.01 |
| UpdateReQuery2xReturnExistingAsync | 1000000 | 10              | 0.5                 |   611.9 ms |    NA |  1.05 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 1000000 | 10              | 0.9                 |   609.5 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 1000000 | 10              | 0.9                 |   577.6 ms |    NA |  0.95 |
| UpdateReQueryAsync                 | 1000000 | 10              | 0.9                 |   605.2 ms |    NA |  0.99 |
| UpdateReQueryReturnsExistingAsync  | 1000000 | 10              | 0.9                 |   577.4 ms |    NA |  0.95 |
| UpdateReQuery2xAsync               | 1000000 | 10              | 0.9                 |   737.6 ms |    NA |  1.21 |
| UpdateReQuery2xReturnExistingAsync | 1000000 | 10              | 0.9                 |   582.4 ms |    NA |  0.96 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 1000000 | 100             | 0                   | 5,532.3 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 1000000 | 100             | 0                   | 5,340.8 ms |    NA |  0.97 |
| UpdateReQueryAsync                 | 1000000 | 100             | 0                   | 5,384.6 ms |    NA |  0.97 |
| UpdateReQueryReturnsExistingAsync  | 1000000 | 100             | 0                   | 5,074.3 ms |    NA |  0.92 |
| UpdateReQuery2xAsync               | 1000000 | 100             | 0                   | 5,004.3 ms |    NA |  0.90 |
| UpdateReQuery2xReturnExistingAsync | 1000000 | 100             | 0                   | 5,347.1 ms |    NA |  0.97 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 1000000 | 100             | 0.1                 | 5,192.5 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 1000000 | 100             | 0.1                 | 4,582.6 ms |    NA |  0.88 |
| UpdateReQueryAsync                 | 1000000 | 100             | 0.1                 | 4,348.0 ms |    NA |  0.84 |
| UpdateReQueryReturnsExistingAsync  | 1000000 | 100             | 0.1                 | 4,353.9 ms |    NA |  0.84 |
| UpdateReQuery2xAsync               | 1000000 | 100             | 0.1                 | 4,357.1 ms |    NA |  0.84 |
| UpdateReQuery2xReturnExistingAsync | 1000000 | 100             | 0.1                 | 4,226.2 ms |    NA |  0.81 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 1000000 | 100             | 0.5                 | 4,884.5 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 1000000 | 100             | 0.5                 | 4,257.1 ms |    NA |  0.87 |
| UpdateReQueryAsync                 | 1000000 | 100             | 0.5                 | 4,213.5 ms |    NA |  0.86 |
| UpdateReQueryReturnsExistingAsync  | 1000000 | 100             | 0.5                 | 4,046.8 ms |    NA |  0.83 |
| UpdateReQuery2xAsync               | 1000000 | 100             | 0.5                 | 4,088.5 ms |    NA |  0.84 |
| UpdateReQuery2xReturnExistingAsync | 1000000 | 100             | 0.5                 | 3,995.5 ms |    NA |  0.82 |
|                                    |         |                 |                     |            |       |       |
| UpdateForceAsync                   | 1000000 | 100             | 0.9                 | 4,331.8 ms |    NA |  1.00 |
| UpdateForceReturnExistingAsync     | 1000000 | 100             | 0.9                 | 4,174.2 ms |    NA |  0.96 |
| UpdateReQueryAsync                 | 1000000 | 100             | 0.9                 | 4,306.1 ms |    NA |  0.99 |
| UpdateReQueryReturnsExistingAsync  | 1000000 | 100             | 0.9                 | 4,370.2 ms |    NA |  1.01 |
| UpdateReQuery2xAsync               | 1000000 | 100             | 0.9                 | 4,309.6 ms |    NA |  0.99 |
| UpdateReQuery2xReturnExistingAsync | 1000000 | 100             | 0.9                 | 4,362.7 ms |    NA |  1.01 |
