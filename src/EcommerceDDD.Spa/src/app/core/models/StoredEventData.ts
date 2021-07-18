export class StoredEventData {
  id: string;
  action: string;
  timestamp: string;
  user: string;

  constructor(id: string, action: string, timestamp: string, user: string) {
    this.id = id;
    this.action = action;
    this.timestamp = timestamp;
    this.user = user;
  }
}

export class StoredEventDataRow {
  columnName: string;
  position: number;

  constructor(columnName: string, position: number) {
    this.columnName = columnName;
    this.position = position;
  }
}
