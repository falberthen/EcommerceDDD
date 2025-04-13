import { Component, OnInit, inject } from '@angular/core';
import { faDollarSign, faEuroSign } from '@fortawesome/free-solid-svg-icons';
import { LOCAL_STORAGE_ENTRIES } from '@ecommerce/constants/appConstants';
import { LocalStorageService } from '@core/services/local-storage.service';
import { CurrencyNotificationService } from '@ecommerce/services/currency-notification.service';

@Component({
    selector: 'app-currency-dropdown',
    templateUrl: './currency-dropdown.component.html',
    styleUrls: ['./currency-dropdown.component.css'],
    standalone: false
})
export class CurrencyDropdownComponent implements OnInit {
  private localStorageService = inject(LocalStorageService);
  private notificationService = inject(CurrencyNotificationService);

  faDollarSign = faDollarSign;
  faEuroSign = faEuroSign;
  currentCurrency!: string;

  ngOnInit() {
    this.notificationService.currentCurrency.subscribe(
      (currencyCode) => (this.currentCurrency = currencyCode)
    );
    var storedCurrency = this.localStorageService.getValueByKey(
      LOCAL_STORAGE_ENTRIES.storedCurrency
    );

    if (!storedCurrency)
      this.localStorageService.setValue(
        LOCAL_STORAGE_ENTRIES.storedCurrency,
        LOCAL_STORAGE_ENTRIES.defaultCurrency
      );
  }

  setCurrency(currency: string) {
    this.localStorageService.setValue(
      LOCAL_STORAGE_ENTRIES.storedCurrency,
      currency
    );
    this.notificationService.changeCurrency(currency);
  }

  getCurrentCurrency() {
    return this.localStorageService.getValueByKey(
      LOCAL_STORAGE_ENTRIES.storedCurrency
    );
  }
}
