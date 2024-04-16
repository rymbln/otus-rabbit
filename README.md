# OtusRabbit

## RabbitMQ. �������������� � ���������

### �������� ��������� � ������ Exchange

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

### �������� �� Exchange � ���������� ���������

```json
{
  "exchange": "logs",
  "routingKey": "",
  "body": "Log message"
}
```

### �������� �� Exchange � ���������

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

### �������� �� Exchange � ��������� �� �������

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

### �������� � ��������� ������

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