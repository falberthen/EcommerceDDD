import { Component, OnInit } from '@angular/core';
import { faDollarSign, faEuroSign } from '@fortawesome/free-solid-svg-icons';
import { appConstants } from 'src/app/modules/ecommerce/constants/appConstants';
import { CurrencyNotificationService } from 'src/app/modules/ecommerce/services/currency-notification.service';
import { LocalStorageService } from 'src/app/core/services/local-storage.service';

@Component({
  selector: 'app-currency-dropdown',
  templateUrl: './currency-dropdown.component.html',
  styleUrls: ['./currency-dropdown.component.css'],
})
export class CurrencyDropdownComponent implements OnInit {
  faDollarSign = faDollarSign;
  faEuroSign = faEuroSign;
  currentCurrency!: string;

  constructor(
    private localStorageService: LocalStorageService,
    private notificationService: CurrencyNotificationService
  ) {}

  ngOnInit() {
    this.notificationService.currentCurrency.subscribe(
      (currencyCode) => (this.currentCurrency = currencyCode)
    );
    var storedCurrency = this.localStorageService.getValueByKey(
      appConstants.storedCurrency
    );

    if (!storedCurrency)
      this.localStorageService.setValue(
        appConstants.storedCurrency,
        appConstants.defaultCurrency
      );
  }

  setCurrency(currency: string) {
    this.localStorageService.setValue(appConstants.storedCurrency, currency);
    this.notificationService.changeCurrency(currency);
  }

  getCurrentCurrency() {
    return this.localStorageService.getValueByKey(appConstants.storedCurrency);
  }
}
