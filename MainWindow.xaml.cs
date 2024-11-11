using System;
using System.Windows;


namespace MemberRegistry
{
    public partial class MainWindow : Window
    {
        private MongoDbService mongoDbService;

        public MainWindow()
        {
            InitializeComponent();
            mongoDbService = new MongoDbService();
            LoadMembers(); 

        }

        private async void AddMemberButton_Click(object sender, RoutedEventArgs e)
        {
            var newMember = new Member
            {
                FirstName = FirstNameTextBox.Text,
                LastName = LastNameTextBox.Text,
                Address = AddressTextBox.Text,
                DateOfBirth = DateOfBirthPicker.SelectedDate.Value,
                Email = EmailTextBox.Text,
                //generate metodi
                MembershipNumber = GenerateMembershipNumber()
            };

            await mongoDbService.AddMemberAsync(newMember);  // Lisää jäsen
            MessageBox.Show("Jäsen lisätty!");

            LoadMembers();  
        }

        //generoi jäsnenumero
        private string GenerateMembershipNumber()
        {
            return Guid.NewGuid().ToString().Substring(0, 8).ToUpper();  
        }

        private async void LoadMembers()
        {
            // Haetaan jäsenet MongoDB:stä ja asetetaan ne ListBoxiin
            var members = await mongoDbService.GetAllMembersAsync();
            MembersListBox.ItemsSource = members;
        }

        private async void EditMemberButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedMember == null)
            {
                MessageBox.Show("Valitse ensin jäsen muokattavaksi.");
                return;
            }
                      
            selectedMember.FirstName = FirstNameTextBox.Text;
            selectedMember.LastName = LastNameTextBox.Text;
            selectedMember.Address = AddressTextBox.Text;
            selectedMember.DateOfBirth = DateOfBirthPicker.SelectedDate.Value;
            selectedMember.Email = EmailTextBox.Text;

            // Tallennetaan muutokset tietokantaan
            await mongoDbService.UpdateMemberAsync(selectedMember);
            MessageBox.Show("Jäsenen tiedot päivitetty.");
            LoadMembers();
        }

        private async void DeleteMemberButton_Click(object sender, RoutedEventArgs e)
        {
            if (MembersListBox.SelectedItem == null)
            {
                MessageBox.Show("Valitse jäsen jonka tiedot poistetaan.");
                return;
            }

            var selectedMember = (Member)MembersListBox.SelectedItem;
            await mongoDbService.DeleteMemberAsync(selectedMember.Id);
            LoadMembers();  // Päivitetään jäsenlista
        }
        private Member selectedMember;

        private void MembersListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedMember = (Member)MembersListBox.SelectedItem;

         
            if (selectedMember != null)
            {
                FirstNameTextBox.Text = selectedMember.FirstName;
                LastNameTextBox.Text = selectedMember.LastName;
                AddressTextBox.Text = selectedMember.Address;
                DateOfBirthPicker.SelectedDate = selectedMember.DateOfBirth;
                EmailTextBox.Text = selectedMember.Email;
            }
        }




    }
}
