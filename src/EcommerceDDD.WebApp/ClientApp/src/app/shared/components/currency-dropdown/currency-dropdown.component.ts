import { Component, OnInit } from '@angular/core';
import { faDollarSign } from '@fortawesome/free-solid-svg-icons';
import { appConstants } from 'app/core/constants/appConstants';
import { CurrencyNotificationService } from 'app/core/services/currency-notification.service';
import { LocalStorageService } from 'app/core/services/local-storage.service';

@Component({
  selector: 'app-currency-dropdown',
  templateUrl: './currency-dropdown.component.html',
  styleUrls: ['./currency-dropdown.component.css']
})
export class CurrencyDropdownComponent implements OnInit {

  faDollarSign = faDollarSign;
  currentCurrency: string;

  constructor(private localStorageService: LocalStorageService,
    private notificationService: CurrencyNotificationService) { }

  ngOnInit() {

    this.notificationService.currentCurrency.subscribe(currencyCode => this.currentCurrency = currencyCode)
    var storedCurrency = this.localStorageService.getValueByKey(appConstants.storedCurrency);

    if(!storedCurrency)
      this.localStorageService.setValue(appConstants.storedCurrency, appConstants.defaultCurrency);
  }

  setCurrency(currency: string) {
    this.localStorageService.setValue(appConstants.storedCurrency, currency);
    this.notificationService.changeCurrency(currency)
  }

  getCurrentCurrency(){
    return this.localStorageService.getValueByKey(appConstants.storedCurrency);
  }
}
