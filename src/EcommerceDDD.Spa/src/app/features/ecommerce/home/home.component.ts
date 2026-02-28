import { Component } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faBook, faFileAlt, faTachometerAlt, faDatabase, faStream } from '@fortawesome/free-solid-svg-icons';


@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  imports: [FontAwesomeModule]
})
export class HomeComponent {
    faBook = faBook;
    faFileAlt = faFileAlt;
    faTachometerAlt = faTachometerAlt;
    faStream = faStream;
    faDatabase = faDatabase;
}
