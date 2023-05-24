## Реализовано
Сделаны все компоненты в базовом варианте. 

## Запуск
Для запуска нужно прописать connection strings во всех appsetings:
- notifications/appsettings.json
- backend/api/appsettings.json
- auth/api/appsettings.json
- admin-panel/mvc/appsettings.json

Миграции должны поставиться автоматически. Если не поставились, они лежат в:
- backend/dal/Migrations
- auth/dal/Migrations

Для работы notification необходим rabbitmq, где нужно заранее создать очередь, имя которой прописать в appsettings.json в секции RabbitConfig:NotificationQueueName.
Так же необходимо указать ClientOrigin для работы cors.

После запуска компонента auth, в бд должен появиться пользователь с ролью администратора и кредами: **admin@admin / admin**. Его можно использовать для входа в админ панель