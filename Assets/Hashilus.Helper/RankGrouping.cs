using System;
using System.Collections.Generic;
using System.Linq;

public class RankGroup<T>
{
    public readonly int rank;
    public T[] players;

    public RankGroup(int rank, T[] players)
    {
        this.rank = rank;
        this.players = players;
    }
}

public static class RankGrouping
{
    public static RankGroup<T>[] GetRankGroups<T>(IEnumerable<T> players, Func<T, int> getScore, bool scoreAscending = false, bool rankAscending = true)
    {
        var scoreGroups = GetScoreGroups(players, getScore, scoreAscending);
        var rankGroups = scoreGroups.SelectMany((ps, i) => ps.Select(p => new { player = p, rank = i + 1 }))
                                    .GroupBy(g => g.rank, g => g.player)
                                    .Select(g => new RankGroup<T>(g.Key, g.ToArray()));

        if (rankAscending)
        {
            return rankGroups.OrderBy(g => g.rank).ToArray();
        }
        else
        {
            return rankGroups.OrderByDescending(g => g.rank).ToArray();
        }
    }

    static IOrderedEnumerable<IGrouping<int, T>> GetScoreGroups<T>(IEnumerable<T> players, Func<T, int> getScore, bool scoreAscending)
    {
        var scoreGroups = players
                                .Select(p => new { player = p, score = getScore(p) })
                                .GroupBy(p => p.score, p => p.player);
        if (scoreAscending)
        {
            return scoreGroups.OrderBy(g => g.Key);
        }
        else
        {
            return scoreGroups.OrderByDescending(g => g.Key);
        }
    }
}