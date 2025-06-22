# dominate.io Backend part #

## Создание лобби ##

1. POST **/api/Lobby** - создание лобби (передаются кол-во игроков от 2 до 4 и поле)
    - Пример запроса:
      ```json
      {
       "playersCount": 2,
       "field": [
       {
         "q": 0,
         "r": 0,
         "s": 0,
         "power": 5,
         "owner": "Player1",
         "size": true
       },
       {
         "q": 1,
         "r": -1,
         "s": 0,
         "power": 3,
         "owner": null,
         "size": false
       }
      ]
      }
      ```
    - Пример ответа:
      ```json
       {
        "code": "ABCD12"
       }
       ```
2. GET **/api/Lobby/ABCD12** - проверяет существуют ли лобби с таким кодом
    - Пример ответа:
      ```json
       {
         "isExist": true
       }
       ```

## Управление лобби и игровой процесс ##

WS **/api/Game?code=ABCD12&nickname=Player1**

---
**ВАЖНО!** Если WS разорвал соединение, значит произошла ошибка.

types:

1. **Join** - присоединиться к комнате
    - Пример запроса:
       ```json
         {
          "type": "Join"
         }
       ```
    - Пример ответа:
        - Отправляется всем:
           ```json
            {
              "type": "PlayerJoined",
              "nickname": "Player1",
              "color": "Red"
            }
          ```
2. **GetPlayers** - возвращает всех игроков из лобби
    - Пример запроса:
        ```json
          {
           "type": "GetPlayers"
          }
        ```
    - Пример ответа:
        - Отправляется игроку:
            ```json
             {
              "type": "SendPlayers",
              "players": {
                "Player1": {
                  "Color": "Red",
                  "IsReady": false
                  }
                }
              }
            ```
3. **SwitchReadiness** - изменяет готовность игрока ("с готов" на "не готов" и наоборот).
    - Пример запроса:
   ```json
      {
       "type": "SwitchReadiness"
      }
   ```
    - Пример ответа:
        - Отправляется всем:
   ```json
   {
      "type": "Readiness",
      "nickname": "Player1",
      "isReady": true
   }
   ```

**P.S. когда все игроки готовы, сервер отправляет сообщение о начале игры с полем и последовательностью ходов, пример:**

```json
   {
  "type": "GameStarted",
  "playersQueue": [
    "Player1",
    "Player2"
  ],
  "field": [
    {
      "q": 1,
      "r": -1,
      "s": 0,
      "power": 3,
      "owner": "Player2",
      "size": false
    },
    {
      "q": 0,
      "r": 0,
      "s": 0,
      "power": 5,
      "owner": "Player1",
      "size": true
    }
  ]
}
```

4. **MakeMove** - позваимодействовать с клеткой (фазы захвата и улучшения), передаются изменненные ячейки
    - Пример запроса:
      ```json
         {
           "type": "MakeMove",
           "moves": [{
             "q": 1,
             "r": -1,
             "s": 0,
             "power": 5,
             "owner": "Player2",
             "size": false
            }
           ]
         }
       ```
    - Пример ответа (отправляется всем):
   ```json
      {
       "type": "MoveMade",
       "nickname": "Player2",
        "moves": [
          {
            "q": 1,
            "r": -1,
            "s": 0,
            "power": 5,
            "owner": "Player2",
            "size": false
          }
        ],
        "message": "Correct move"
      }
   ```
**P.S. Если после хода какой-то игрок выиграл или проиграл, то до сообщение о сделанном ходе выводятся подобные сообщения:**
```json
  {
    "type": "PlayerLost",
    "loser": "Player1"
  }
```
```json
  {
    "type": "GameEnd",
    "winner": "Player1"
  }
```
5. **PhaseEnd** - закончить фазу атаки
   - Пример запроса:
   ```json
      {
       "type": "PhaseEnd"
      }
   ```
   - Пример ответа:
      - Отправляется всем:
   ```json
   {
      "type": "PhaseEnd",
      "nickname": "Player1",
      "message": "Player Player1 end attack phase, upgrade now"
   }
   ```
6. **TurnEnd** - закончить ход
   - Пример запроса:
   ```json
      {
       "type": "TurnEnd"
      }
   ```
   - Пример ответа:
      - Отправляется всем:
   ```json
   {
      "type": "TurnEnd",
      "nickname": "Player1",
      "nextPlayer": "Player2",
      "message": "Player Player1 end his(her) turn, next is Player2"
   }
   ```
7. **Leave** - покинуть игру
   - Пример запроса:
   ```json
      {
       "type": "Leave"
      }
   ```
   - Пример ответа (если игра не началась):
      - Отправляется всем:
   ```json
   {
      "type": "PlayerLeft",
      "nickname": "Player1"
   }
   ```
   - Пример ответа (если игра началась):
   - Отправляется всем ("nextPlayer" - чей следующий ход (может совпадать с уже текущим игроком)):
      ```json
      {
         "type": "PlayerLeft",
         "nickname": "Player1",
         "nextPlayer": "Revava"
      }
      ```