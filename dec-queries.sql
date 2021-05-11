SELECT [t].[Id], [t].[Name], [c].[Id], [c].[SessionId], [c].[Text], [c].[Voters], [p].[Id], [p].[SessionId], [p].[Username]
FROM (
    SELECT TOP(1) [s].[Id], [s].[Name]
    FROM [Sessions] AS [s]
    WHERE [s].[Id] = 31
) AS [t]
LEFT JOIN [Choices] AS [c] ON [t].[Id] = [c].[SessionId]
LEFT JOIN [Participants] AS [p] ON [t].[Id] = [p].[SessionId]
ORDER BY [t].[Id], [c].[Id], [p].[Id]