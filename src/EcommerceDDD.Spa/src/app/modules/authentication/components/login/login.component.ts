import { Component, OnInit, inject } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '@core/services/auth.service';
import { LoaderService } from '@core/services/loader.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  returnUrl!: string;

  private router = inject(Router);
  private formBuilder = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private loaderService = inject(LoaderService);
  private authenticationService = inject(AuthService);


  ngOnInit() {
    this.authenticationService.currentUser && this.router.navigate(['/home']);
    this.loginForm = this.formBuilder.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
    });

    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  get isLoading() {
    return this.loaderService.loading$;
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return field!.invalid && field!.touched;
  }

  async onSubmit() {
    // stop here if form is invalid
    if (this.loginForm.invalid) {
      return;
    }

    await firstValueFrom(
      this.authenticationService.login(
        this.formControls.email.value,
        this.formControls.password.value
      )
    ).then(() => {
      this.router.navigate([this.returnUrl]);
    });
  }

  // getter for easy access to form fields
  private get formControls() {
    return this.loginForm.controls;
  }
}
