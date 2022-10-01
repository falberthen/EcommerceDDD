export class StoredEventData {
  aggregateId: string;
  eventTypeName!: string;
  eventData!: string;

  constructor(aggregateId: string, eventTypeName: string, eventData: string) {
    this.aggregateId = aggregateId;
    this.eventTypeName = eventTypeName;
    this.eventData = eventData;
  }
}
