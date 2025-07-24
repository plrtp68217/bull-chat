COMPOSE=docker compose -f docker-compose.yml
MIGRATE_FILE=-f docker-compose.migrate.yml

# Сборка всех образов
build:
	$(COMPOSE) build

# Применение миграций
migrate:
	$(COMPOSE) $(MIGRATE_FILE) run --rm migrate

# Запуск всех сервисов
up:
	$(COMPOSE) up

# Запуск с пересборкой
up-build:
	$(COMPOSE) up --build

# Остановка и удаление контейнеров
down:
	$(COMPOSE) down

# Полная очистка (контейнеры + тома)
clean:
	$(COMPOSE) down -v

# Логи
logs:
	$(COMPOSE) logs -f

# Перезапуск webapp
restart:
	$(COMPOSE) restart webapp
