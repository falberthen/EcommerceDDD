import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerDetailsComponent } from './customer-details.component';
import { CustomersService } from '@ecommerce/services/customers.service';
import { NotificationService } from '@core/services/notification.service';
import { AuthService } from '@core/services/auth.service';
import { of } from 'rxjs';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

describe('CustomerProfileComponent', () => {
  let component: CustomerDetailsComponent;
  let fixture: ComponentFixture<CustomerDetailsComponent>;

  beforeEach( () => {
     TestBed.configureTestingModule({
      declarations: [CustomerDetailsComponent],
      providers: [
        {provide: CustomersService, useValue: {loadCustomerDetails: jest.fn().mockReturnValue(of([]))}}, 
        {provide: NotificationService, useValue: {}}, 
        {provide: AuthService, useValue: {}}
      ], 
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomerDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
