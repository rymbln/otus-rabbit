# OtusRabbit

## RabbitMQ. Взаимодействие с очередями

### Отправка сообщения в пустой Exchange

```json
{
  "exchange": "",
  "routingKey": "hello",
  "body": "Message to empty exchange with routing key"
}
```

```json
{
  "exchange": "",
  "routingKey": "task_queue",
  "body": "Message for multiple consumers"
}
```

### Подписка на Exchange с временными очередями

```json
{
  "exchange": "logs",
  "routingKey": "",
  "body": "Log message"
}
```

### Подписка на Exchange с роутингом

```json
{
  "exchange": "direct_logs",
  "routingKey": "info",
  "body": "Information message"
}
```

```json
{
  "exchange": "direct_logs",
  "routingKey": "error",
  "body": "Error message"
}
```

### Подписка на Exchange с роутингом по топикам

```json
{
  "exchange": "topic_logs",
  "routingKey": "api.error",
  "body": "Error form api"
}
```

```json
{
  "exchange": "topic_logs",
  "routingKey": "api.info",
  "body": "Information message from api"
}
```

```json
{
  "exchange": "topic_logs",
  "routingKey": "client.any",
  "body": "Just a message"
}
```

### Отправка с ожиданием ответа

```json
{
  "body": "This.is.a.message.with.multiple.dots"
}
```

## MassTransit

```json
{
  "from": "Me",
  "to": "Friend",
  "message": "Hello darkness, my old friend I've come to talk with you again"
}
```