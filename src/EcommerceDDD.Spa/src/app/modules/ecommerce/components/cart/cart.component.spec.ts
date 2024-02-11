import { ComponentFixture, TestBed, async } from '@angular/core/testing';
import {HttpClientTestingModule} from '@angular/common/http/testing'

import { CartComponent } from './cart.component';
import { AuthService } from '@core/services/auth.service';
import { ConfirmationDialogService } from '@core/services/confirmation-dialog.service';
import { NotificationService } from '@core/services/notification.service';

describe('OrderSummaryComponent', () => {
  let component: CartComponent;
  let fixture: ComponentFixture<CartComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CartComponent],
      imports: [HttpClientTestingModule], 
      providers: [
        {provide: AuthService, useValue: {}}, 
        {provide: ConfirmationDialogService, useValue: {}}, 
        {provide: NotificationService, useValue: {}}
      ]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
