/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrdersComponent } from './orders.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { AuthService } from '@core/services/auth.service';

describe('OrdersComponent', () => {
  let component: OrdersComponent;
  let fixture: ComponentFixture<OrdersComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [OrdersComponent],
      imports: [HttpClientTestingModule], 
      providers: [{provide: AuthService, useValue: {}}]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(OrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
