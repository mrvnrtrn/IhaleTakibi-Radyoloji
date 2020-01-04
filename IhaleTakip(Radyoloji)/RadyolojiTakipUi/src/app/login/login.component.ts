import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from '../entity/service/authentication.service';
import { User } from '../entity/model/user';
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html'
})

export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  loading: boolean = false;
  submitted: boolean = false;
  returnUrl: string;
  error: string = "";

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private authenticationService: AuthenticationService
  ) {
    localStorage.removeItem('currentUser');
    // redirect to home if already logged in
    // if (this.authenticationService.currentUserValue) {
    //   this.router.navigate(["/"]);
    // }
  }


  ngOnInit() {
    localStorage.removeItem('currentUser');

    this.loginForm = this.formBuilder.group({
      username: ["", Validators.required],
      password: ["", Validators.required]
    });
    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams["returnUrl"] || "/";
  }

  // convenience getter for easy access to form fields
  get f(): FormGroup["controls"] {
    return this.loginForm.controls;
  }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.loginForm.invalid) {
      return;
    }

    this.loading = true;
    this.authenticationService
      .login(this.f.username.value, this.f.password.value)
      .pipe(first())
      .subscribe(
        (user: User) => {

          console.log(this.returnUrl);
          this.router.navigate(['/index.html']);
        },
        error => {
          this.error = error;
          this.loading = false;
        }
      );
  }
}
