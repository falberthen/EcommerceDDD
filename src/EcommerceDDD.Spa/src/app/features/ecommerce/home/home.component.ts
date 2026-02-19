import { Component } from '@angular/core';
import { faBook, faFileAlt, faTachometerAlt } from '@fortawesome/free-solid-svg-icons';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html'
})
export class HomeComponent {
    faBook = faBook;
    faFileAlt = faFileAlt;
    faTachometerAlt = faTachometerAlt;
}
