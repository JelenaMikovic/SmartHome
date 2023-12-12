import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthService } from 'src/services/auth.service';
import { CodeDialogComponent } from '../code-dialog/code-dialog.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-register-admin',
  templateUrl: './register-admin.component.html',
  styleUrls: ['./register-admin.component.css']
})
export class RegisterAdminComponent implements OnInit {

  isVisible : boolean = false;
  filePath: string = "";
  file: File = {} as File;

  registerForm = new FormGroup({
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [Validators.required]),
    name: new FormControl('', [Validators.required]),
    surname: new FormControl('', [Validators.required]),
  })

  constructor(
    private dialog: MatDialog,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
  }

  register(){
    if (this.registerForm.valid) {
      const form = {
        name: this.registerForm.get('name')?.value,
        surname: this.registerForm.get('surname')?.value,
        email: this.registerForm.get('email')?.value,
        password: this.registerForm.get('password')?.value,
        role: "ADMIN",
        //image: this.filePath
      };

      this.authService.register(form).subscribe(
        (success) => {
          this.router.navigate(["/home"]);
        },
        (error) => {
          console.error('Login error:', error);
          this.snackBar.open('An error occurred while registering in', 'Close', { duration: 3000 });
        }
      );
    } else {
      this.snackBar.open('Please fill in all required fields', 'Close', { duration: 3000 });
    }
  }

  onFileSelect(event: any) {
    event.preventDefault();
    if (event.target.files){
      var reader = new FileReader();
      this.file = event.target.files[0];
      reader.readAsDataURL(this.file);
      reader.onload=(e: any)=>{
        event.preventDefault();
        this.filePath = reader.result as string;
      }
    }
  }
  
}
