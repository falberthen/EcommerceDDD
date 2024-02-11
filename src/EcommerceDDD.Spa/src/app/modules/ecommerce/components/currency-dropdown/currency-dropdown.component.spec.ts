import { ComponentFixture, TestBed } from '@angular/core/testing';


import { CurrencyDropdownComponent } from './currency-dropdown.component';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

describe('CurrencyDropdownComponent', () => {
  let component: CurrencyDropdownComponent;
  let fixture: ComponentFixture<CurrencyDropdownComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CurrencyDropdownComponent],
      schemas: [CUSTOM_ELEMENTS_SCHEMA]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CurrencyDropdownComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
