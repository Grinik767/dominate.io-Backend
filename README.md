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
         "owner": "Player2",
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
