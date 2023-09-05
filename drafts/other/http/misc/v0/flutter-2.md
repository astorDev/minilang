```ruby
GameFilter
    ?int difficulty
    ?int players
    ?int minTime
    ?int maxTime

    matches =>
        if difficulty && @Game.game.difficulty != difficulty
            return false
        
        if players && (game.minPlayers > players || game.maxPlayers < players)
            return false

        if minTime && game.minTime < minTime
            return false

        if maxTime && game.maxTime < maxTime
            return false

        return true

StatefulWidget GameList
    List_Game games
    createState => gameListState
```

```ruby
State_GameList GameListState
    build => 
        return scaffold
            appBar
                text 'Vataga Games'
                actions = 
                [
                    iconButton 
                        icon ICON_SEARCH
                        => showSearch @context (vatagaSearchDelegate widget.games)
                ]
            padding
                edgeInsetsWith left=20 right=20
                column
                    crossAxisAlignment = CrossAxisAlignment>start
                    [
                        filters
                        futureBuilder
                            (getGames context)
                            =>
                                if @snapshot.hasData
                                    return gamesListing List_Game.@snapshot.data
                                return circularProgressIndicator
                    ]

getGames async => 
    return await httpGet (parseUri 'http://vataga.com/games') 
        -> jsonDecode List.$.body 
        -> .map *gameFromJson 
        -> .toList
```

```ruby
filters => 
    return wrap
        spacing = 20
        [
            filter
                [
                    "Any difficulty"
                    "Easy"
                    "Medium"
                    "Hard"
                ],
                => mapDifficultyText @
        ]
```