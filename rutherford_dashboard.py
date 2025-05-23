import streamlit as st
import pandas as pd
import sqlite3

# Connect to the database
conn = sqlite3.connect("scholarship.db")
df = pd.read_sql("SELECT * FROM StudentGrades", conn)

# Scholarship logic
def calculate_custom_award(course_scores):
    REQUIRED_LANGUAGES = {"English 30-1", "English 30-2", "Franais 30-1", "Franais 30-2"}
    REQUIRED_SOCIAL = {"Social Studies 30-1", "Social Studies 30-2"}

    lang_course = max((c for c in REQUIRED_LANGUAGES if c in course_scores), key=lambda c: course_scores[c], default=None)
    social_course = max((c for c in REQUIRED_SOCIAL if c in course_scores), key=lambda c: course_scores[c], default=None)

    if not lang_course:
        return "❌ Ineligible - No valid English/French course"
    if not social_course:
        return "❌ Ineligible - No valid Social Studies course"

    remaining = {k: v for k, v in course_scores.items() if k not in {lang_course, social_course}}
    top_remaining = sorted(remaining.values(), reverse=True)[:3]

    if len(top_remaining) < 3:
        return "❌ Ineligible - Fewer than 5 eligible courses"

    avg = (course_scores[lang_course] + course_scores[social_course] + sum(top_remaining)) / 5

    if avg >= 80:
        return f"✅ Eligible for $2500 - Average: {avg:.2f}"
    elif avg >= 75:
        return f"✅ Eligible for $1500 - Average: {avg:.2f}"
    else:
        return f"❌ Ineligible - Average: {avg:.2f}"

# Title
st.title("🎓 Welcome to Rutherford Scholarship Checker")

# Display student names only
st.subheader("📋 List of Students")
st.write(", ".join(sorted(df["StudentName"].unique())))

st.info("To use scholarship checker, start below:")

# Sidebar filters
st.sidebar.title("Filters")
clear = st.sidebar.button("Clear Filter Selections")

# Initialize filter values
filter_mode_default = "All" if clear else st.session_state.get("filter_mode", "All")
selected_grade = None
selected_names = []

# Select filter mode
filter_mode = st.sidebar.radio("Filter by:", ["All", "Grade Level", "Select Names"], index=["All", "Grade Level", "Select Names"].index(filter_mode_default))
st.session_state["filter_mode"] = filter_mode

# Filtered dataframe logic
filtered_df = pd.DataFrame()
if filter_mode == "All" and not clear:
    filtered_df = df.copy()

elif filter_mode == "Grade Level":
    grade_levels = sorted(df["GradeLevel"].unique())
    selected_grade = None if clear else st.sidebar.selectbox("Choose Grade:", grade_levels)
    if selected_grade:
        filtered_df = df[df["GradeLevel"] == selected_grade]

elif filter_mode == "Select Names":
    name_options = sorted(df["StudentName"].unique())
    selected_names = [] if clear else st.sidebar.multiselect("Select Student(s):", name_options)
    if selected_names:
        filtered_df = df[df["StudentName"].isin(selected_names)]

if not filtered_df.empty:
    for _, row in filtered_df.iterrows():
        st.markdown(f"### 🎯 {row['StudentName']} ({row['GradeLevel']})")
        student_courses = {col: row[col] for col in row.index if col not in ["StudentID", "StudentName", "GradeLevel"] and pd.notna(row[col])}

        with st.expander("View Courses and Grades"):
            st.write(pd.DataFrame.from_dict(student_courses, orient="index", columns=["Grade"]))

            selected_courses = st.multiselect(f"Select 5 or more courses for {row['StudentName']}:", list(student_courses.keys()), key=row['StudentName'])
            if len(selected_courses) >= 5:
                selected_scores = {course: student_courses[course] for course in selected_courses}
                result = calculate_custom_award(selected_scores)
                st.success(result)
            else:
                st.info("Please select at least 5 courses to evaluate eligibility.")

conn.close()
